using BypassEmote.EmoteSwap;
using BypassEmote.Enums;
using BypassEmote.Helpers;
using BypassEmote.Models;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using NoireLib;
using NoireLib.Animations.Helpers;
using NoireLib.Helpers;
using NoireLib.Hooking;
using System;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.EmoteController;

namespace BypassEmote;

public partial class Service
{
    public delegate void OnEmoteFuncDelegate(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2);

    public static NoireHook<OnEmoteFuncDelegate>? OnEmoteHook;
    public static NoireHook<AgentEmote.Delegates.ExecuteEmote> AgentExecuteEmoteHook;
    public static NoireHook<EmoteManager.Delegates.ExecuteEmote> ExecuteEmoteHook;
    public static NoireHook<RaptureHotbarModule.Delegates.ExecuteSlot>? ExecuteHotbarSlotHook;

    private const byte HotbarSlotNotExecuted = 0;
    private static bool inHotbarSlot;

    // The hotbar entry point the client actually calls on a press. It is a tail-call thunk sitting at
    // RaptureHotbarModule.ExecuteSlotById + 0x22, and it takes a HotbarSlot*, which is the signature the
    // detour below is written against.
    //
    // From the Chinese 7.56 build on, FFXIVClientStructs' RaptureHotbarModule.Delegates.ExecuteSlot no longer
    // resolves to that thunk: its member function pattern matches the address of ExecuteSlotById itself, whose
    // real parameters are (uint hotbarId, uint slotId). Hooking that address with a HotbarSlot* signature both
    // misses every press, because the client enters through the thunk, and hands the original a bogus third
    // argument. Scanning for the thunk ourselves is client agnostic: this pattern was verified to be unique and
    // to point at the right address on both the global 7.55 and the Chinese 7.56 binaries.
    private const string ExecuteSlotSignature = "E9 ?? ?? ?? ?? 73 25 8B CA 49 8D 91 A0 00 00 00";

    private static unsafe void InstallHooks()
    {
        AgentExecuteEmoteHook = new(DetourAgentExecuteEmote, true);
        ExecuteEmoteHook = new(DetourExecuteEmote, true);

        ExecuteHotbarSlotHook = InstallExecuteSlotHook();

        try
        {
            // From https://github.com/RokasKil/EmoteLog/blob/master/EmoteLog/Hooks/EmoteReaderHook.cs#L11
            OnEmoteHook = new("E8 ?? ?? ?? ?? 48 8D 8B ?? ?? ?? ?? 4C 89 74 24", OnEmoteDetour, true, "OnEmote");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "OnEmote Hook error");
        }
    }

    // Bypassing emotes from the hotbar is a convenience. If the entry point cannot be found the plugin must
    // still load and every other feature must keep working, so a failure here is logged instead of thrown.
    private static unsafe NoireHook<RaptureHotbarModule.Delegates.ExecuteSlot>? InstallExecuteSlotHook()
    {
        try
        {
            var bySignature = new NoireHook<RaptureHotbarModule.Delegates.ExecuteSlot>(
                ExecuteSlotSignature, DetourExecuteHotbarSlot, true, "ExecuteSlot");

            Log.Debug("Hotbar ExecuteSlot hook bound by signature.");

            return bySignature;
        }
        catch (Exception ex)
        {
            Log.Warning($"The ExecuteSlot signature '{ExecuteSlotSignature}' did not resolve: {ex.Message}");
        }

        try
        {
            var byClientStructs = new NoireHook<RaptureHotbarModule.Delegates.ExecuteSlot>(
                DetourExecuteHotbarSlot, true);

            Log.Warning("The hotbar ExecuteSlot hook fell back to the client structs delegate. On a client where "
                + "that delegate points at ExecuteSlotById instead of the ExecuteSlot thunk, hotbar bypassing "
                + "will not intercept presses.");

            return byClientStructs;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "The hotbar ExecuteSlot hook could not be installed; hotbar bypassing stays off.");
            return null;
        }
    }

    private static void HandleEmote(Emote emote)
    {
        var chara = NoireService.ObjectTable.LocalPlayer;

        if (chara == null)
            return;

        var isEmoteUnlocked = EmoteHelper.IsEmoteUnlocked(emote.RowId);

        if (isEmoteUnlocked)
            return;

        EmotePlayer.PlayEmote(chara, emote);
    }

    private static unsafe byte DetourExecuteHotbarSlot(RaptureHotbarModule* thisPtr, RaptureHotbarModule.HotbarSlot* hotbarSlot)
    {
        if (Configuration.SelfBypassMode == SelfBypassMode.EmoteSwap && TrySwapHotbarSlot(hotbarSlot))
            return HotbarSlotNotExecuted;

        byte ret;

        inHotbarSlot = true;

        try
        {
            // The detour can only run while the hook exists, so the hook is never null in here.
            ret = ExecuteHotbarSlotHook!.Original(thisPtr, hotbarSlot);
        }
        finally
        {
            inHotbarSlot = false;
        }

        if (Configuration.SelfBypassMode == SelfBypassMode.EmoteSwap)
            return ret;

        if (!Configuration.PluginEnabled || !Configuration.BypassOnHotbarSlotTriggered)
            return ret;

        if (hotbarSlot->CommandType != RaptureHotbarModule.HotbarSlotType.Emote)
            return ret;

        var emoteId = hotbarSlot->CommandId;
        var emote = EmoteHelper.GetEmoteById(emoteId);

        if (emote == null)
            return ret;

        HandleEmote(emote.Value);

        return ret;
    }

    private static unsafe bool TrySwapHotbarSlot(RaptureHotbarModule.HotbarSlot* hotbarSlot)
    {
        if (!Configuration.PluginEnabled || !Configuration.BypassOnHotbarSlotTriggered)
            return false;

        if (hotbarSlot == null || hotbarSlot->CommandType != RaptureHotbarModule.HotbarSlotType.Emote)
            return false;

        if (NoireService.ObjectTable.LocalPlayer == null)
            return false;

        if (EmoteHelper.GetEmoteById(hotbarSlot->CommandId) is not { } emote)
            return false;

        if (IsPoseFamilySource(emote.RowId)) // Might be unnecessary since poses are all unlocked by default, but whatever
            return false;

        if (LeaveToTheGame(emote.RowId))
            return false;

        // Only swallow the press when the swap really took the emote over. A refused or failed pipeline has to
        // leave the press to the game rather than eating it and doing nothing.
        return Orchestrator?.TrySwap(emote) == true;
    }

    private static void ArmCacheBreak(ushort emoteId)
    {
        if (!Configuration.PluginEnabled || !Configuration.AlwaysCacheBreak)
            return;

        if (Configuration.SelfBypassMode != SelfBypassMode.EmoteSwap || NoireService.ClientState.IsGPosing)
            return;

        if (Orchestrator is not { IsExecutingSwap: false } || Rebinder is not { } rebinder
            || NoireService.ObjectTable.LocalPlayer == null)
        {
            return;
        }

        if (!LeaveToTheGame(emoteId) || IsPoseFamilySource(emoteId) || Catalog?.Get(emoteId) is not { } emote)
            return;

        try
        {
            rebinder.Arm(emote);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Cache break for emote {emoteId} failed; the press is left to the game.");
        }
    }

    // A /cpose cycle member, or Change Pose itself
    private static bool IsPoseFamilySource(uint emoteRowId)
        => Catalog?.Get(emoteRowId)?.IsPoseFamily == true;

    // Every emote source (chat command, game macro line, emote window) lands here
    // It also lands here before any unlock check, so this is better than hooking the ExecuteCommand function
    private static unsafe void DetourAgentExecuteEmote(
        AgentEmote* agent, ushort emoteId, PlayEmoteOption* playEmoteOption, bool addToHistory, bool liveUpdateHistory)
    {
        // A hotbar press already went through the slot hook
        var emote = inHotbarSlot ? null : ResolveSelfEmote(emoteId);

        CloseContextMenu();

        if (emote.HasValue && Configuration.SelfBypassMode == SelfBypassMode.EmoteSwap
            && !IsPoseFamilySource(emote.Value.RowId))
        {
            if (!LeaveToTheGame(emote.Value.RowId))
            {
                Orchestrator?.TrySwap(emote.Value);
                return;
            }
        }

        AgentExecuteEmoteHook.Original(agent, emoteId, playEmoteOption, addToHistory, liveUpdateHistory);

        // Direct Play
        if (emote.HasValue && Configuration.SelfBypassMode != SelfBypassMode.EmoteSwap)
            HandleEmote(emote.Value);
    }

    private static Emote? ResolveSelfEmote(ushort emoteId)
        => NoireService.ObjectTable.LocalPlayer == null ? null : EmoteHelper.GetEmoteById(emoteId);

    // Detour the execute emote function to stop any currently playing bypassed looping emotes before executing a new base/obtained game emote
    // Necessary since emote bypassing will prevent the player from executing any base/obtained emote otherwise
    private static unsafe bool DetourExecuteEmote(EmoteManager* emoteManager, ushort emoteId, PlayEmoteOption* playEmoteOption)
    {
        DropSwappedIdlePose(emoteId);

        if (Orchestrator is { } orchestrator && SwapMods?.ArmedFor(emoteId) is { } armed
            && ShouldEndSwapBeforeExecuting(Configuration.SelfBypassMode, orchestrator.IsExecutingSwap, armed, emoteId)
            && Configuration.SwapLifetime != SwapLifetime.Never)
        {
            EndWatcher?.StopWatching();

            if (Catalog?.Get(emoteId) is { } released)
                Rebinder?.Arm(released);

            SwapMods.DeselectEntry(armed);
        }
        else
        {
            ArmCacheBreak(emoteId);
        }

        var chara = NoireService.ObjectTable.LocalPlayer;

        if (chara == null)
            return ExecuteEmoteHook.Original(emoteManager, emoteId, playEmoteOption);

        var trackedCharacter = CommonHelper.TryGetTrackedCharacterFromAddress(chara.Address);

        if (trackedCharacter != null)
        {
            var emote = EmoteHelper.GetEmoteById(emoteId);
            if (emote.HasValue)
            {
                var emoteCategory = EmoteHelper.GetEmoteCategory(emote.Value);
                if (emoteCategory != NoireLib.Enums.EmoteCategory.Expressions)
                    EmotePlayer.StopLoop(chara, true);
            }
        }

        return ExecuteEmoteHook.Original(emoteManager, emoteId, playEmoteOption);
    }

    private static void DropSwappedIdlePose(ushort emoteId)
    {
        if (Configuration.SelfBypassMode != SelfBypassMode.EmoteSwap)
            return;

        if (SwapMods?.ArmedIdlePose() is not { } idlePose)
            return;

        EndWatcher?.StopWatchingIdlePose();
        SwapMods.DeselectEntry(idlePose);

        var redrawn = SwapOrchestrator.IdlePoseNeedsRedrawOnEnd(idlePose.IdlePoseIndex)
            && Penumbra?.RedrawLocalPlayer() == true;

        Log.Debug($"Emote {emoteId} is being played, dropping swapped idle pose. "
            + (redrawn ? "Character redrawn." : ""));
    }

    internal static bool ShouldEndSwapBeforeExecuting(SelfBypassMode mode, bool isExecutingSwap,
        SwapOptionEntry? armed, ushort emoteId)
        => mode == SelfBypassMode.EmoteSwap
        && !isExecutingSwap
        && armed != null
        && armed.TargetEmote == emoteId;

    // Hooking this function to detect when an emote is played by any character (including the local player)
    // This is necessary if a player is playing a bypassed looping emote and then tries to play
    // a base/obtained game emote. In that case, we need to stop the bypassed looping emote first.
    // Only for direct play mode
    private static void OnEmoteDetour(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2)
    {
        try
        {
            var character = CharacterHelper.GetCharacterFromAddress((nint)instigatorAddr);

            if (character != null)
            {
                var trackedCharacter = CommonHelper.TryGetTrackedCharacterFromAddress(character.Address);

                if (trackedCharacter == null)
                    return;

                var emote = EmoteHelper.GetEmoteById(emoteId);

                if (!emote.HasValue || EmoteHelper.GetEmoteCategory(emote.Value) != NoireLib.Enums.EmoteCategory.Expressions)
                    EmotePlayer.StopLoop(character, true);

                if (emote != null)
                    EmotePlayer.PlayEmote(character, emote.Value);
            }
        }
        finally
        {
            OnEmoteHook?.Original(unk, instigatorAddr, emoteId, targetId, unk2);
        }
    }
}
