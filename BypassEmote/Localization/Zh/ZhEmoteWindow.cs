namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>UI/EmoteWindow.cs</c>（主窗口）。 </summary>
/// <remarks>
/// "Open settings" 与 "Support me" 已由 <see cref="ZhCommon"/> 提供，此处不再重复。
/// </remarks>
internal static class ZhEmoteWindow
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // 窗口标题与标题栏按钮
        ("Bypass Emote - Locked Emotes", "Bypass Emote - 未解锁的情感动作"),
        ("Show every logs", "显示全部日志"),
        ("Show changelogs", "显示更新日志"),
        ("Join the Discord", "加入 Discord"),

        // 列表上方的开关与搜索
        ("Show all emotes", "显示全部情感动作"),
        ("Show invalid emotes", "显示无效情感动作"),
        ("Show Invalid Emotes", "显示无效情感动作"),
        ("Show IDs", "显示 ID"),
        ("Search emotes...", "搜索情感动作..."),

        // 分类标签
        ("All", "全部"),
        ("General", "一般"),
        ("Special", "特殊"),
        ("Expressions", "表情"),
        ("Other", "其他"),
        ("Fav", "收藏"),
        ("Blocked", "屏蔽"),

        // 空列表提示
        ("No favorited emote", "没有收藏的情感动作"),
        ("No blocked emote", "没有屏蔽的情感动作"),

        // 列表中的悬浮说明与提示
        ("Blocked: no swap will land on this emote", "已屏蔽：替换不会作用于此情感动作"),
        ("Block this emote as a swap target", "将此情感动作屏蔽为替换目标"),
        ("Patch: {0}", "版本：{0}"),
        ("Left-click to apply to yourself", "左键点击对自己使用"),
        ("Right-click for more options", "右键点击查看更多选项"),
        ("Drag onto a hotbar slot to assign it", "拖到热键栏格子上即可分配"),

        // 右键菜单
        ("Force swap", "强制替换"),
        ("Only Emote Swap mode swaps.", "只有表情替换模式才会替换。"),
        ("Apply emote on Minion", "对宠物播放情感动作"),
        ("Apply emote on Pet", "对召唤兽播放情感动作"),
        ("Apply emote on Chocobo", "对陆行鸟播放情感动作"),
        ("Add an override...", "添加覆写..."),
        ("Assign emote to Hotbar...", "将情感动作分配到热键栏..."),
        ("Create a mod from this emote...", "用此情感动作创建模组..."),

        // 底部支持栏
        ("Support me on Ko-fi", "在 Ko-fi 上支持我"),
        ("This plugin is free and always will be, donations are appreciated.", "本插件永久免费，欢迎捐赠支持。"),
        ("Help, bug reports and updates.", "获取帮助、反馈问题与更新。"),

        // 收藏 / 屏蔽 快捷添加
        ("Add an emote to your favorites...", "将情感动作添加到收藏..."),
        ("(favorite)", "（收藏）"),
        ("Block an emote as a swap target...", "将情感动作屏蔽为替换目标..."),
        ("(blocked)", "（屏蔽）"),

        // 顶部工具栏
        ("Refresh Locked Emotes", "刷新未解锁的情感动作"),
        ("Create a mod", "创建模组"),
        ("Sync...", "同步..."),
        ("Sync All (/be syncall)", "同步全部 (/be syncall)"),
        ("Sync BE users (/be sync)", "同步 BE 用户 (/be sync)"),
        ("Sync all (/be syncall)", "同步全部 (/be syncall)"),

        // 聊天消息（与 Plugin.Commands.cs 共用同一文案）
        ("No minion summoned.", "没有召唤宠物。"),
        ("No pet summoned.", "没有召唤召唤兽。"),
        ("No chocobo summoned.", "没有召唤陆行鸟。"),
    ];
}
