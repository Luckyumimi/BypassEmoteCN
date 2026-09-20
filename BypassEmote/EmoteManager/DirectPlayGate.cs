using BypassEmote.Enums;
using BypassEmote.Localization;
using NoireLib.Animations.Helpers;
using NoireLib.Enums;

namespace BypassEmote;

/// <summary>
/// Whether Direct Play may play an animation or not based on if unsafe mod is enabled
/// </summary>
internal static class DirectPlayGate
{
    // Shown both in the settings notice and as a chat error, so it is translated here.
    internal static readonly string SafeModeMessage = L.T(
        "Due to detectability, you need to be in the base pose (pose 0) of your current stance to bypass emotes "
        + "in safe mode.");

    internal const string SafeModeRefusalKind = "directplay.safemode";

    internal static bool IsSafeState(byte poseIndex, EmoteCondition condition, int slotIndex)
        => poseIndex == 0
        || (condition is EmoteCondition.SittingInChair or EmoteCondition.SittingOnGround
            && IsAdditiveSlot(slotIndex));

    internal static bool IsAdditiveSlot(int slotIndex)
        => slotIndex is ActionTimelineSlots.UpperBody or ActionTimelineSlots.Adjust;

    internal static bool ShouldBlockSelfPlay(
        SelfBypassMode mode, bool unsafeEnabled, bool isLocalPlayer, bool isSafeState)
        => mode == SelfBypassMode.DirectPlay && !unsafeEnabled && isLocalPlayer && !isSafeState;
}
