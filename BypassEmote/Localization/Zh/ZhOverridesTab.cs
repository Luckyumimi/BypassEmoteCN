namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>UI/OverridesTab.cs</c>（「情感动作覆写」标签页）。 </summary>
/// <remarks> "The emote catalog is still building." 已由 <see cref="ZhEmotePoolTab"/> 提供，此处不再重复。 </remarks>
internal static class ZhOverridesTab
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // ── 标签页说明 ────────────────────────────────────────────────────────
        ("This tab allows you to make sure a locked emote will always use one or multiple specific target emotes.",
            "此标签页让你指定某个未解锁的情感动作始终使用一个或多个特定的目标情感动作。"),

        // ── 左侧：来源情感动作 ────────────────────────────────────────────────
        ("Override an emote...", "覆写一个情感动作..."),
        ("(overridden)", "（已覆写）"),
        ("No override yet.", "尚无覆写。"),
        ("Remove this override", "移除此覆写"),
        ("Pick an emote on the left.", "请先在左侧选择一个情感动作。"),

        // 键中的两个前导空格是调用点原文的一部分
        ("  (empty)", "（空）"),

        // ── 右侧：目标情感动作 ────────────────────────────────────────────────
        ("Add a target emote...", "添加目标情感动作..."),
        ("(already a target)", "（已是目标）"),
        ("No target yet.", "尚无目标。"),
        ("Limited to selected targets only", "仅限选定的目标"),
        ("On, this emote lands on one of these targets or it does not play at all.\nOff, the usual configuration takes over whenever none of them can be used as targets during a swap.",
            "开启：该情感动作只会落到这些目标中的一个，否则完全不播放。\n关闭：替换时若这些目标都不可用，则恢复使用常规配置。"),
    ];
}
