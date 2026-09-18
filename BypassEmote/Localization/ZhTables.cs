using System;
using System.Collections.Generic;

namespace BypassEmote.Localization;

/// <summary>
/// Collects every <c>Zh*</c> table into the lookup used by <see cref="L"/>.
/// <para/>
/// Each area owns one file under <c>Localization/Zh/</c>, so translations can be added
/// without two people ever touching the same file.
/// </summary>
internal static class ZhTables
{
    /// <summary> How many entries were declared twice with a different translation. Diagnostic only. </summary>
    internal static int ConflictCount { get; private set; }

    internal static void Register(Dictionary<string, string> table)
    {
        Add(table, ZhCommon.Entries);
        Add(table, ZhConfigWindow.Entries);
        Add(table, ZhEmoteWindow.Entries);
        Add(table, ZhEmotePoolTab.Entries);
        Add(table, ZhCreateModWindow.Entries);
        Add(table, ZhOverridesTab.Entries);
        Add(table, ZhWindows.Entries);
        Add(table, ZhDebugWindow.Entries);
        Add(table, ZhCommands.Entries);
        Add(table, ZhChat.Entries);
        Add(table, ZhSwapAdvice.Entries);
        Add(table, ZhSafety.Entries);
        Add(table, ZhChangelog.Entries);
    }

    private static void Add(Dictionary<string, string> table, (string En, string Zh)[] entries)
    {
        foreach (var (english, chinese) in entries)
        {
            if (string.IsNullOrEmpty(english) || string.IsNullOrEmpty(chinese))
                continue;

            if (table.TryAdd(english, chinese))
                continue;

            if (!string.Equals(table[english], chinese, StringComparison.Ordinal))
                ConflictCount++;
        }
    }
}
