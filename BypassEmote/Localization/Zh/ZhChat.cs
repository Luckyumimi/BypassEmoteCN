namespace BypassEmote.Localization;

/// <summary> 中文文案：聊天消息、交换反馈、错误与警告、右键菜单。 </summary>
internal static class ZhChat
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // ── ModActionChatPayloads：聊天栏里的模组操作链接与回执 ──────────────────
        ("[Open]", "[打开]"),
        ("[Disable]", "[禁用]"),
        ("{0} Mod not opened.", "{0} 未打开模组。"),
        ("Penumbra would not open '{0}'.", "Penumbra 无法打开「{0}」。"),
        ("{0} Mod not disabled.", "{0} 未禁用模组。"),
        ("No Penumbra collection is assigned to your character. Mod not disabled.",
            "你的角色没有分配 Penumbra 集合，未禁用模组。"),
        ("Penumbra would not switch '{0}' off in {1}.", "Penumbra 无法在 {1} 中关闭「{0}」。"),
        ("'{0}' is now off in {1}.", "「{0}」现已在 {1} 中关闭。"),

        // ── EmotePlayer：直接播放被拒绝时的聊天回执 ────────────────────────────
        ("Bypass Emote is waiting for you to choose how it should play locked emotes.",
            "Bypass Emote 正在等待你选择如何处理未解锁的情感动作。"),
        ("You cannot bypass this emote right now.", "你现在无法无视限制播放该情感动作。"),

        // ── CommonHelper：情感动作没有名字时的占位名 ───────────────────────────
        ("No name", "无名称"),
    ];
}
