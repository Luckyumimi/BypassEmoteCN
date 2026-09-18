#if DEBUG
using BypassEmote.EmoteSwap;
using BypassEmote.Enums;
using BypassEmote.Helpers;
using BypassEmote.IPC;
using BypassEmote.IPC.Enums;
using BypassEmote.Models;
using BypassEmote.Safety;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoireLib;
using NoireLib.Helpers;
using NoireLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BypassEmote.UI;

public class DebugWindow : Window, IDisposable
{

    private uint selectedEmoteId = 0;
    private string emoteSearchText = string.Empty;
    private List<Emote>? cachedEmoteList = null;

    public DebugWindow() : base(L.T("Bypass Emote Debug") + "###BypassEmote")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(float.MinValue, float.MaxValue),
        };

        IpcProvider.OnReady += LogReady;
        IpcProvider.OnStateChange += LogStateChanged;
        IpcProvider.OnStateChangeImmediate += LogStateChangedImmediate;
    }

    public override void Draw()
    {
        // The window name doubles as the ImGui id, so only the visible half is translated.
        var windowTitle = L.T("Bypass Emote Debug") + "###BypassEmote";

        if (!string.Equals(WindowName, windowTitle, StringComparison.Ordinal))
            WindowName = windowTitle;

        using (ImRaii.TabBar("DebugTabs"))
        {
            using (var tab = ImRaii.TabItem(L.T("IPC Tests")))
            {
                if (tab)
                    DrawIpcTestsTab();
            }

            using (var tab = ImRaii.TabItem(L.T("Tracked Characters")))
            {
                if (tab)
                    DrawTrackedCharactersTab();
            }

#if DEBUG
            using (var tab = ImRaii.TabItem(L.T("Network Relay")))
            {
                if (tab)
                    DrawNetworkRelayTab();
            }
#endif

            using (var tab = ImRaii.TabItem(L.T("Swap Layers")))
            {
                if (tab)
                    DrawSwapLayers();
            }

            using (var tab = ImRaii.TabItem(L.T("Emote Pool")))
            {
                if (tab)
                    EmotePoolTab.Draw();
            }

            using (var tab = ImRaii.TabItem(L.T("Kept Swaps")))
            {
                if (tab)
                    DrawKeptSwapsTab();
            }

            using (var tab = ImRaii.TabItem(L.T("Patch Approval")))
            {
                if (tab)
                    DrawPatchApprovalTab();
            }

        }
    }

    private static string approvalNote = string.Empty;

    private static string approvalNotice = string.Empty;

    private static void DrawPatchApprovalTab()
    {
        if (Service.PatchApproval is not { } gate)
        {
            ImGui.TextUnformatted(L.T("The gate is not up."));
            return;
        }

        ImGui.TextUnformatted(L.T("Probed client: {0}", GameClientHelper.Name(GameClientHelper.Detected())));
        ImGui.TextUnformatted(L.T("Gate reads: {0}", GameClientHelper.Name(gate.Client)));

        ImGui.Separator();

        DrawPretendToggle(L.T("Pretend this is the Korean client"), GameClient.Korean);
        DrawPretendToggle(L.T("Pretend this is the Chinese client"), GameClient.Chinese);
        DrawPretendToggle(L.T("Pretend this client cannot be identified"), GameClient.Unknown);

        var rejecting = IPCCaller_Penumbra.PretendIdentifierRejected;

        if (ImGui.Checkbox(L.T("Pretend Penumbra rejects the player identifier (KR/CN clients debug)"), ref rejecting))
            IPCCaller_Penumbra.PretendIdentifierRejected = rejecting;

        if (rejecting && GameClientHelper.Current() is not (GameClient.Korean or GameClient.Chinese))
        {
            ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f),
                L.T("Client is Global. Enable one of the client checkboxes above."));
        }

        if (Service.Penumbra?.PlayerCollectionFallbackSource is { } fallbackSource)
            ImGui.TextUnformatted(L.T("Fallback in use: {0}", fallbackSource));

        if (rejecting && Service.Penumbra is { } penumbraGateway)
        {
            ImGui.TextUnformatted(L.T("Assignment chain:"));
            ImGui.TextUnformatted(penumbraGateway.DescribePlayerAssignmentChain());
        }

        ImGui.Separator();

        ImGui.TextUnformatted(L.T("Game build: {0}", gate.GameVersion));
        ImGui.TextUnformatted(L.T("Status: {0}", gate.Status));
        ImGui.TextWrapped(L.T("Reason: {0}", gate.Reason));

        if (gate.Notice is { Length: > 0 } notice)
            ImGui.TextWrapped(L.T("Notice: {0}", notice));

        ImGui.TextUnformatted(L.T("Governs: {0}    Holds hooks: {1}    Held: {2}",
            gate.Governs, gate.HoldsHooks, gate.HeldCount));

        var due = gate.LastCheckedUtc is { } checkedUtc
            ? (checkedUtc + PatchApprovalGate.RetryInterval).ToLocalTime().ToString("HH:mm:ss")
            : L.T("as soon as the loop runs");

        ImGui.TextUnformatted(L.T("Next automatic check: {0}", due));

        ImGui.Separator();

        if (ImGui.Button(L.T("Drop the recorded approval")))
            gate.Forget();

        ImGui.SameLine();
        ImGuiComponents.HelpMarker(L.T("Clears the approval stored in the config and at runtime"));

        ImGui.InputTextWithHint("##BypassEmoteApprovalNotice", L.T("Notice for this build"),
            ref approvalNotice, 512);

        if (ImGui.Button(L.T("Approve this build in the .json file")))
            approvalNote = ApprovalPublisher.Publish(gate.GameVersion, gate.PluginVersion, gate.Client, approvalNotice);

        if (approvalNote.Length > 0)
            ImGui.TextWrapped(approvalNote);
    }

    private static void DrawPretendToggle(string label, GameClient client)
    {
        var pretending = GameClientHelper.Forced == client;

        if (ImGui.Checkbox(label, ref pretending))
            GameClientHelper.Forced = pretending ? client : null;
    }

    private static void DrawKeptSwapsTab()
    {
        var manager = Service.SwapMods;

        if (manager == null)
        {
            ImGui.TextUnformatted(L.T("The swap mod manager is not initialized."));
            return;
        }

        var registry = manager.Registry;
        var directory = manager.ModDirectoryName;

        ImGui.TextUnformatted(L.T("Mod directory: {0}",
            directory.Length == 0 ? L.T("<no character loaded>") : directory));
        ImGui.TextUnformatted(L.T("Penumbra: {0}", DescribeModState(manager.PenumbraState())));
        ImGui.TextUnformatted(L.T("Drawn body: {0}", registry.Skeleton ?? L.T("<unknown>")));
        ImGui.TextUnformatted(L.T("Swap files: {0}", DescribeSize(manager.SwapFilesSize())));

        var clearing = ImGui.Button(L.T("Clear kept swaps") + "##KeptSwaps");

        ImGui.Separator();

        if (registry.Entries.Count == 0)
        {
            ImGui.TextUnformatted(L.T("No swap is kept."));
        }
        else
        {
            DrawKeptSwapsTable(manager, registry.Entries);
        }

        if (clearing)
        {
            manager.ForgetAll();
            Service.Orchestrator?.ResetDispatchMemory();
        }
    }

    private static void DrawKeptSwapsTable(SwapModManager manager, IReadOnlyList<SwapOptionEntry> entries)
    {
        using var table = ImRaii.Table("##KeptSwapsTable", 8,
            ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp
            | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoSavedSettings);

        if (!table)
            return;

        ImGui.TableSetupColumn(L.T("Target"));
        ImGui.TableSetupColumn(L.T("Source"));
        ImGui.TableSetupColumn(L.T("Option"));
        ImGui.TableSetupColumn(L.T("Races"), ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn(L.T("Last used"), ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn(L.T("Selected"), ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn(L.T("Rules"), ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableSetupColumn("##KeptSwapsActions", ImGuiTableColumnFlags.WidthFixed);
        ImGui.TableHeadersRow();

        var currentRules = SwapRulesStamp.Current();

        (SwapOptionEntry Entry, bool Select)? pending = null;

        for (var index = 0; index < entries.Count; index++)
        {
            var entry = entries[index];

            ImGui.TableNextRow();

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(EmoteLabel(entry.TargetEmote) + (entry.IsIdlePoseSwap ? " " + L.T("(idle pose)") : string.Empty));

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(EmoteLabel(entry.SourceEmote));

            ImGui.TableNextColumn();
            ImGui.TextUnformatted($"{entry.GroupName} / {entry.OptionName}");

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(entry.FilesByRace.Count.ToString());

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(string.Join(", ", entry.FilesByRace.Keys));

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(entry.LastUsedStamp.ToString());

            ImGui.TableNextColumn();
            ImGui.TextUnformatted(entry.SelectedByUs ? L.T("yes") : L.T("no"));

            ImGui.TableNextColumn();

            if (ImGui.Button(L.T("Select") + $"##KeptSwap{index}"))
                pending = (entry, true);

            ImGui.SameLine();

            using (ImRaii.Disabled(!entry.SelectedByUs))
            {
                if (ImGui.Button(L.T("Turn off") + $"##KeptSwap{index}"))
                    pending = (entry, false);
            }
        }

        if (pending is not { } action)
            return;

        if (action.Select)
            manager.SelectExisting(action.Entry);
        else
            manager.DeselectEntry(action.Entry);
    }

    private static string DescribeModState(ModState? state)
    {
        if (state is not { } held)
            return L.T("does not hold the mod");

        return L.T("{0}, priority {1}",
            held.Enabled ? L.T("enabled") : L.T("disabled"), held.Priority);
    }

    private static string DescribeSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";

        if (bytes < 1024 * 1024)
            return $"{bytes / 1024f:0.#} KB";

        return $"{bytes / (1024f * 1024f):0.##} MB";
    }

    private static string EmoteLabel(uint emoteRowId)
    {
        if (emoteRowId == 0)
            return "-";

        return EmoteHelper.GetEmoteById(emoteRowId) is { } emote
            ? $"{CommonHelper.GetEmoteName(emote)} (#{emoteRowId})"
            : $"#{emoteRowId}";
    }

#if DEBUG
    private void DrawNetworkRelayTab()
    {
        var relay = Service.Networker;

        if (relay == null)
        {
            ImGui.Text(L.T("Network relay is not initialized."));
            return;
        }

        ImGui.Text(L.T("Network: {0}", relay.NetworkName));
        ImGui.Text(L.T("State: {0}", relay.State) + (relay.IsHub ? " " + L.T("(hub)") : string.Empty));
        ImGui.Text(L.T("Self: {0}", relay.SelfId));

        var isActive = relay.IsActive;
        if (ImGui.Checkbox(L.T("Active"), ref isActive))
            relay.SetActive(isActive);

        ImGui.SameLine();

        var enableLan = relay.Options.EnableLan;
        if (ImGui.Checkbox(L.T("LAN discovery"), ref enableLan))
        {
            relay.Options.EnableLan = enableLan;

            if (relay.IsActive)
                relay.SetActive(false).SetActive(true);
        }

        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(L.T("Instances on the same PC find each other with no configuration.\n"
                + "LAN discovery reaches other PCs, and may need a Windows Firewall inbound allow."));

        var localPlayer = NoireService.ObjectTable.LocalPlayer;
        if (localPlayer != null)
            relay.Self.Set("character", localPlayer.Name.TextValue);

        ImGui.Text(L.T("Peers:"));
        using (var child = ImRaii.Child("##NetworkPeers", new Vector2(-1, -1), true))
        {
            if (child)
            {
                ImGui.Text(L.T("You - {0} [{1}]", relay.Self, relay.IsHub ? L.T("hub") : L.T("client")));

                foreach (var peer in relay.OtherPeers)
                    ImGui.Text($"{peer} [{(peer.IsSameMachine ? L.T("same PC") : L.T("LAN"))}]");
            }
        }
    }
#endif

    private static void DrawSwapLayers()
    {
        LayerSwitch(L.T("Swap owned emotes") + "##SwapLayer", SwapLayers.SwapOwnedEmotes,
            value => SwapLayers.SwapOwnedEmotes = value);

        ImGui.SameLine();
        ImGuiComponents.HelpMarker(L.T("Sends emotes you already own through the swap instead of letting the game play them, for debugging only."));

        LayerSwitch(L.T("Weapon in hand") + "##SwapLayer", SwapLayers.WeaponInHand,
            value => SwapLayers.WeaponInHand = value);

        LayerSwitch(L.T("Weapon stow at end") + "##SwapLayer", SwapLayers.WeaponStowAtEnd,
            value => SwapLayers.WeaponStowAtEnd = value);

        LayerSwitch(L.T("Weapon travel animation") + "##SwapLayer", SwapLayers.WeaponTravelAnimation,
            value => SwapLayers.WeaponTravelAnimation = value);
    }

    private static void LayerSwitch(string label, bool current, Action<bool> write)
    {
        var value = current;
        if (!ImGui.Checkbox(label, ref value))
            return;

        write(value);
    }

    private void InitCache()
    {
        if (cachedEmoteList == null)
        {
            var emoteSheet = ExcelSheetHelper.GetSheet<Emote>();
            if (emoteSheet != null)
            {
                cachedEmoteList = emoteSheet
                    .Where(e => CommonHelper.GetEmotePlayType(e) != EmotePlayType.DoNotPlay)
                    .Where(e => CommonHelper.IsEmoteDisplayable(e))
                    .OrderByDescending(e => e.RowId)
                    .ToList();
            }
            else
            {
                cachedEmoteList = new List<Emote>();
            }
        }
    }

    private void DrawIpcTestsTab()
    {
        ImGui.Text(L.T("IPC Version: {0}", IpcProvider.ApiVersion().ToString()));
        ImGui.Text(L.T("IPC Is Ready: {0}", IpcProvider.IsReady()));

        ImGui.Separator();

        InitCache();

        ImGui.Text(L.T("Select Emote:"));

        var selectedEmoteName = selectedEmoteId == 0 ? L.T("None") : GetEmoteDisplayName(selectedEmoteId);

        ImGui.SetNextItemWidth(250);

        using (var combo = ImRaii.Combo("##EmoteCombo", selectedEmoteName, ImGuiComboFlags.HeightRegular))
        {
            if (combo)
            {
                ImGui.SetNextItemWidth(-1);
                ImGui.InputTextWithHint("##EmoteSearch", L.T("Search emotes..."), ref emoteSearchText, 256);

                bool isNoneSelected = selectedEmoteId == 0;
                if (ImGui.Selectable(L.T("None"), isNoneSelected))
                {
                    selectedEmoteId = 0;
                }

                foreach (var emote in cachedEmoteList)
                {
                    var emoteName = CommonHelper.GetEmoteName(emote);
                    var emoteId = emote.RowId;

                    if (!string.IsNullOrWhiteSpace(emoteSearchText) &&
                        !emoteName.Contains(emoteSearchText, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var initialPosY = ImGui.GetCursorPosY();

                    var iconSize = 25f;
                    try
                    {
                        var iconTex = NoireService.TextureProvider.GetFromGameIcon(Helpers.CommonHelper.GetEmoteIcon(emote));
                        var wrap = iconTex?.GetWrapOrEmpty();
                        if (wrap != null)
                        {
                            var posY = ImGui.GetCursorPosY();
                            ImGui.Image(wrap.Handle, new Vector2(iconSize, iconSize));
                            ImGui.SameLine();
                            ImGui.SetCursorPosY(posY + MathF.Max(0, (iconSize - ImGui.GetTextLineHeight()) * 0.5f));
                        }
                    }
                    catch
                    {
                        // ignore icon issues
                    }

                    bool isSelected = selectedEmoteId == emoteId;
                    if (ImGui.Selectable($"{emoteName}##{emoteId}", isSelected))
                    {
                        selectedEmoteId = emoteId;
                        ImGui.CloseCurrentPopup();
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
            }
        }

        ImGui.SameLine();

        var localPlayer = NoireService.ObjectTable.LocalPlayer;
        var target = CommonHelper.GetLocalTarget();
        var executedAction = selectedEmoteId == 0 ? ExecutedAction.StoppedEmote : ExecutedAction.StartedEmote;
        var currentState = CurrentState.Stopped;

        if (selectedEmoteId != 0)
        {
            var emote = EmoteHelper.GetEmoteById(selectedEmoteId);
            if (emote != null && CommonHelper.GetEmotePlayType(emote.Value) == EmotePlayType.Looped)
                currentState = CurrentState.PlayingEmote;
        }

        if (ImGui.Button(L.T("Set local player state")) && localPlayer != null)
        {
            var characterState = CommonHelper.CreateCharacterState(localPlayer.Address, executedAction, currentState, selectedEmoteId);
            IpcProvider.SetStateForCharacter(localPlayer.Address, characterState.Serialize());
        }

        ImGui.SameLine();

        if (ImGui.Button(L.T("Set target state")) && target != null)
        {
            if (target is not ICharacter castTarget)
                return;
            var characterState = CommonHelper.CreateCharacterState(castTarget.Address, executedAction, currentState, selectedEmoteId);
            IpcProvider.SetStateForCharacter(castTarget.Address, characterState.Serialize());
        }

        if (ImGui.Button(L.T("Clear local player state")) && localPlayer != null)
            IpcProvider.ClearStateForCharacter(localPlayer.Address);

        ImGui.SameLine();

        if (ImGui.Button(L.T("Clear target state")) && target != null)
            IpcProvider.ClearStateForCharacter(target.Address);

        ImGui.Separator();
        ImGui.Text(L.T("Current Local Player IPC Data (Looping only):"));

        var remainingHeight = ImGui.GetContentRegionAvail().Y;
        var heightBlocks = (int)(remainingHeight / 2 - 20);

        using (var child = ImRaii.Child("IpcDataBlockLocal", new Vector2(-1, heightBlocks), true))
        {
            if (child)
            {
                if (localPlayer != null)
                {
                    var ipcData = IpcProvider.GetStateForCharacter(localPlayer.Address);

                    if (!string.IsNullOrEmpty(ipcData))
                    {
                        try
                        {
                            ImGui.TextUnformatted(FormatJson(ipcData));
                        }
                        catch
                        {
                            ImGui.TextUnformatted(ipcData);
                        }
                    }
                }
            }
        }

        ImGui.Separator();
        ImGui.Text(L.T("Current Target IPC Data (Looping only):"));

        using (var child = ImRaii.Child("IpcDataBlockTarget", new Vector2(-1, heightBlocks), true))
        {
            if (child)
            {
                if (CommonHelper.GetLocalTarget() is ICharacter targettedChar)
                {
                    var ipcData = IpcProvider.GetStateForCharacter(targettedChar.Address);

                    if (!string.IsNullOrEmpty(ipcData))
                    {
                        try
                        {
                            ImGui.TextUnformatted(FormatJson(ipcData));
                        }
                        catch
                        {
                            ImGui.TextUnformatted(ipcData);
                        }
                    }
                }
            }
        }
    }

    private void DrawTrackedCharactersTab()
    {
        ImGui.Text(L.T("Tracked Characters:"));

        using (var child = ImRaii.Child("BlockTrackedCharacters", new Vector2(-1, -1), true))
        {
            if (child)
            {
                try
                {
                    var formattedJson = JsonConvert.SerializeObject(EmotePlayer.TrackedCharacters, Formatting.Indented);

                    ImGui.TextUnformatted(formattedJson);
                }
                catch (Exception ex)
                {
                    ImGui.TextUnformatted(L.T("Serialization error: {0}", ex.Message));
                }
            }
        }
    }

    private string GetEmoteDisplayName(uint emoteId)
    {
        if (emoteId == 0) return L.T("None");

        var emote = cachedEmoteList?.FirstOrDefault(e => e.RowId == emoteId);
        if (emote == null) return L.T("Unknown ({0})", emoteId);

        return CommonHelper.GetEmoteName(emote.Value);
    }

    private static string FormatJson(string json)
    {
        return JToken.Parse(json).ToString(Formatting.Indented);
    }

    private void LogReady()
        => Log.Debug("BypassEmote IPC is ready");
    private void LogStateChanged(string liveData, string? cacheData, bool isLocalPlayer)
        => Log.Debug($"BypassEmote IPC sent state changed message. IsLocalPlayer: {isLocalPlayer}, LiveData: {liveData}, CacheData: {cacheData ?? "<null>"}");
    private void LogStateChangedImmediate(string liveData, string? cacheData, bool isLocalPlayer)
        => Log.Debug($"BypassEmote IPC sent immediate state changed message. IsLocalPlayer: {isLocalPlayer}, LiveData: {liveData}, CacheData: {cacheData ?? "<null>"}");

    public void Dispose()
    {
        IpcProvider.OnReady -= LogReady;
        IpcProvider.OnStateChange -= LogStateChanged;
        IpcProvider.OnStateChangeImmediate -= LogStateChangedImmediate;
    }
}
#endif
