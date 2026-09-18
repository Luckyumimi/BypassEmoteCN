namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>Plugin.Commands.cs</c>（聊天命令帮助与用法）与语言指令。 </summary>
internal static class ZhCommands
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // /be
        ("Opens the Bypass Emote main window.", "打开 Bypass Emote 主窗口。"),
        ("Bypasses any emote (including locked ones) on yourself, by command name or ID.",
            "对自己播放任意情感动作（包括未解锁的），可用指令名或 ID。"),
        ("Opens the configuration window.", "打开设置窗口。"),
        ("Syncs only players that are bypassing an emote.", "只同步正在无视限制播放情感动作的玩家。"),
        ("Syncs everyone playing an emote.", "同步所有正在播放情感动作的玩家。"),
        ("Opens the changelog window.", "打开更新日志窗口。"),
        ("Stops the emote currently playing on yourself.", "停止你自己当前播放的情感动作。"),
        ("Exports a zip with the plugin logs and settings, to send to the developer.",
            "导出包含插件日志与设置的 zip 压缩包，用于发送给开发者。"),
        ("Changes the language of the plugin. Usage: /be lang <auto|zh|en>",
            "切换插件语言。用法：/be lang <auto|zh|en>"),
        ("auto, zh or en", "auto、zh 或 en"),
        ("Opens the debug window.", "打开调试窗口。"),
        ("Shows the hooks window.", "显示 Hook 窗口。"),

        // 用法提示
        ("Usage: /be <emote_command> or /be stop", "用法：/be <情感动作指令> 或 /be stop"),
        ("Usage: /bet <emote_command> or /bet stop", "用法：/bet <情感动作指令> 或 /bet stop"),
        ("Usage: /bem <emote_command> or /bem stop", "用法：/bem <情感动作指令> 或 /bem stop"),
        ("Usage: /bep <emote_command> or /bep stop", "用法：/bep <情感动作指令> 或 /bep stop"),
        ("Usage: /bec <emote_command> or /bec stop", "用法：/bec <情感动作指令> 或 /bec stop"),

        // /bet
        ("Applies any emote to a targetted NPC. Only works on NPCs and owned minions/pets. Use /bet <emote_command> or /bet stop.",
            "对选中的 NPC 播放任意情感动作。仅对 NPC 和自己召唤的宠物／召唤兽有效。用法：/bet <情感动作指令> 或 /bet stop。"),
        ("Stops the emote currently playing on your target.", "停止你当前目标正在播放的情感动作。"),
        ("Plays the emote on your target, by command name or ID.", "对目标播放该情感动作，可用指令名或 ID。"),

        // /bem
        ("Applies any emote to your own minion if summoned, without needing to target it. Use /bem <emote_command> or /bem stop.",
            "对自己已召唤的宠物播放任意情感动作，无需选中它。用法：/bem <情感动作指令> 或 /bem stop。"),
        ("Stops the emote currently playing on your minion.", "停止你宠物当前播放的情感动作。"),
        ("Plays the emote on your minion, by command name or ID.", "对宠物播放该情感动作，可用指令名或 ID。"),

        // /bep
        ("Applies any emote to your own pet (carbuncle/eos) if summoned, without needing to target it. Use /bep <emote_command> or /bep stop.",
            "对自己已召唤的召唤兽（宝石兽／艾奥斯）播放任意情感动作，无需选中它。用法：/bep <情感动作指令> 或 /bep stop。"),
        ("Stops the emote currently playing on your pet.", "停止你召唤兽当前播放的情感动作。"),
        ("Plays the emote on your pet, by command name or ID.", "对召唤兽播放该情感动作，可用指令名或 ID。"),

        // /bec
        ("Applies any emote to your own chocobo if summoned, without needing to target it. Use /bec <emote_command> or /bec stop.",
            "对自己已召唤的陆行鸟播放任意情感动作，无需选中它。用法：/bec <情感动作指令> 或 /bec stop。"),
        ("Stops the emote currently playing on your chocobo.", "停止你陆行鸟当前播放的情感动作。"),
        ("Plays the emote on your chocobo, by command name or ID.", "对陆行鸟播放该情感动作，可用指令名或 ID。"),

        // 聊天回执
        ("Error trying to process command", "处理指令时出错"),
        ("No NPC targeted.", "没有选中 NPC。"),
        ("You can only target your own minion, pet, chocobo.", "你只能选中自己的宠物、召唤兽或陆行鸟。"),
        ("No minion summoned.", "没有召唤宠物。"),
        ("No pet summoned.", "没有召唤召唤兽。"),
        ("No chocobo summoned.", "没有召唤陆行鸟。"),
        ("Poses cannot be swapped.", "待机姿势无法被替换。"),
        ("This emote cannot be played in Emote Swap mode.", "该情感动作无法在表情替换模式下播放。"),
        ("Emote not found: {0}\n{1}", "找不到该情感动作：{0}\n{1}"),

        // /be lang
        ("Unknown language \"{0}\". Usage: /be lang <auto|zh|en>",
            "未知的语言“{0}”。用法：/be lang <auto|zh|en>"),
        ("Plugin language: {0}.", "插件语言：{0}。"),
    ];
}