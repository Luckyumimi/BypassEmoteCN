using NoireLib;
using NoireLib.Helpers;
using System.Numerics;

namespace BypassEmote.Helpers;

// Chat payloads (links) to look at a mod that prevented a swap or to switch it off
internal static class ModActionChatPayloads
{
    private static readonly Vector3 LinkColor = ColorHelper.HexToVector3("#4FA3FF");

    private static string PenumbraReason()
        => Service.Penumbra?.UnavailableReason is { Length: > 0 } reason ? reason : L.T("Penumbra is not running.");

    internal static void Append(NoireLogger.ChatMessageBuilder chat, string modDirectory, string modName)
    {
        if (string.IsNullOrEmpty(modDirectory))
            return;

        chat.AddText(" ");
        chat.AddLink(L.T("[Open]"), $"BypassEmote.OpenMod.{modDirectory}", () => Open(modDirectory, modName), LinkColor);
        chat.AddText(" ");
        chat.AddLink(L.T("[Disable]"), $"BypassEmote.DisableMod.{modDirectory}", () => Disable(modDirectory, modName), LinkColor);
    }

    private static void Open(string modDirectory, string modName)
    {
        if (Service.Penumbra is not { Available: true } penumbra)
        {
            LogHelper.Error(L.T("{0} Mod not opened.", PenumbraReason()));
            return;
        }

        if (!penumbra.OpenMod(modDirectory, modName))
            LogHelper.Error(L.T("Penumbra would not open '{0}'.", modName), "modaction.open-failed");
    }

    private static void Disable(string modDirectory, string modName)
    {
        if (Service.Penumbra is not { Available: true } penumbra)
        {
            LogHelper.Error(L.T("{0} Mod not disabled.", PenumbraReason()));
            return;
        }

        if (penumbra.GetPlayerCollection() is not { } collection)
        {
            LogHelper.Error(L.T("No Penumbra collection is assigned to your character. Mod not disabled."));
            return;
        }

        if (!penumbra.TrySetModEnabled(collection.Id, modDirectory, false))
        {
            LogHelper.Error(L.T("Penumbra would not switch '{0}' off in {1}.", modName, collection.Name), "modaction.disable-failed");
            return;
        }

        LogHelper.Info(L.T("'{0}' is now off in {1}.", modName, collection.Name));
    }
}
