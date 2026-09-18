namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>Safety/</c>（补丁审批、发布、门控）。 </summary>
internal static class ZhSafety
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // PatchApproval.Decide —— 这些会显示在设置窗口的审批提示里
        ("The installed game build could not be read.", "无法读取已安装的游戏版本。"),
        ("The approval list could not be reached.", "无法访问审批列表。"),
        ("Game build {0} has not been approved yet.", "游戏版本 {0} 尚未通过审批。"),
        ("Game build {0} names a plugin version the plugin cannot read.", "游戏版本 {0} 指定的插件版本无法被识别。"),
        ("Game build {0} needs Bypass Emote {1} or newer; this is {2}.",
            "游戏版本 {0} 需要 Bypass Emote {1} 或更高版本；当前为 {2}。"),
        ("Game build {0} is approved.", "游戏版本 {0} 已通过审批。"),

        // 未测试客户端说明
        ("The {0} client has not and can not be tested. This plugin might not work and might be unstable/unusable. Please don't use it if it does not work well.",
            "{0} 客户端尚未且无法进行测试。本插件可能无法工作，也可能不稳定／不可用。如果表现不佳，请不要继续使用。"),
        ("This game client is not the Global one, and has not and can not be tested. This plugin might not work and might be unstable/unusable. Please don't use it if it does not work well.",
            "该游戏客户端并非国际服客户端，尚未且无法进行测试。本插件可能无法工作，也可能不稳定／不可用。如果表现不佳，请不要继续使用。"),

        // PatchApprovalGate
        ("Game build {0} was approved earlier.", "游戏版本 {0} 此前已通过审批。"),
        ("Reading the approval list.", "正在读取审批列表。"),
        ("The approval recorded for game build {0} was dropped.", "已清除为游戏版本 {0} 记录的审批结果。"),
        ("The plugin has been approved for this patch. If you noticed weird behaviors prior to this message, try again and it should be fixed now.",
            "本插件已通过该游戏版本的审批。如果你在此提示之前遇到了异常表现，请重试，现在应该已恢复正常。"),
    ];
}