namespace BypassEmote.Localization;

/// <summary> 中文文案：其余小窗口（分配热键、交换提示、建议视图、快捷添加、拖拽）。 </summary>
/// <remarks>
/// "Search emotes..." 在 <see cref="ZhEmoteWindow"/> / <see cref="ZhEmotePoolTab"/> 中已有相同译文，
/// 这里按区域文件的惯例再声明一次；译文一致，因此不会计入冲突。
/// </remarks>
internal static class ZhWindows
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // UI/SwapPromptWindow.cs — 首次启动的模式选择弹窗
        ("Choose how Bypass Emote plays locked emotes", "选择 Bypass Emote 无视限制播放的方式"),
        ("Use Emote Swap", "使用表情替换"),
        ("Keep Direct Play", "保留直接播放"),
        ("A safer way to play locked emotes", "更安全地播放未解锁情感动作"),
        ("Until now Bypass Emote forced the animation onto your character from your own client. Nothing was ever sent to the server, but in specific cases, where your character would be in any pose other than the base one, the game client would send duplicate change pose packets to the server. This is not caused by the plugin itself, but rather by how the game handles pose changes. The \"Idle Animation Delay\" setting in the game's Character Configuration > Control Settings > Character tab is what causes this.",
            "在此之前，Bypass Emote 是在你自己的客户端里把动画强制套用到角色身上的。任何内容都不会发送到服务器，"
            + "但在特定情况下——当你的角色处于基础姿势以外的其他姿势时——游戏客户端会向服务器重复发送变更姿势的封包。"
            + "这并不是插件本身造成的，而是游戏处理姿势变更的方式所致。真正的原因是游戏「角色配置 > 操作设置 > 角色」"
            + "标签页中的「待机动作延迟」设置。"),
        ("The new mode uses Penumbra to swap locked emotes onto unlocked ones. The game itself does the playing, and nothing mismatches between the game and the server anymore.",
            "新模式通过 Penumbra 把未解锁的情感动作替换成已解锁的。播放完全由游戏本身完成，"
            + "游戏与服务器之间不会再出现任何不一致。"),
        ("You can change this at any time in the settings.", "你可以随时在设置中更改。"),

        // UI/AssignHotbarWindow.cs — 绑定到热键栏
        ("Assign", "绑定"),
        ("Assign {0} to hotbar...", "将 {0} 绑定到热键栏..."),
        ("Hotbar", "热键栏"),
        // 下拉选项：10 个普通热键栏 + 8 个十字热键栏，共 18 项，\0 分隔，项数必须与英文一致
        ("1\02\03\04\05\06\07\08\09\010\0XHB 1\0XHB 2\0XHB 3\0XHB 4\0XHB 5\0XHB 6\0XHB 7\0XHB 8",
            "1\02\03\04\05\06\07\08\09\010\0十字热键栏 1\0十字热键栏 2\0十字热键栏 3\0十字热键栏 4\0十字热键栏 5\0十字热键栏 6\0十字热键栏 7\0十字热键栏 8"),
        ("Currently assigned: {0}", "当前已绑定：{0}"),
        ("This slot is empty. You can safely assign an emote.", "此格子为空，可以安全地绑定情感动作。"),
        ("Slot {0} - Empty", "热键栏格子 {0} - 空"),
        ("Slot {0} - {1}", "热键栏格子 {0} - {1}"),
        ("You can also drag and drop an emote from the main window onto any visible hotbar slot.",
            "你也可以把主窗口中的情感动作拖放到任意可见的热键栏格子上。"),

        // UI/EmoteQuickAdd.cs — 快捷添加
        ("Search emotes...", "搜索情感动作..."),
    ];
}
