namespace BypassEmote.Localization;

/// <summary> 中文文案：跨界面复用的通用词。 </summary>
/// <remarks>
/// 本表在 <c>ZhTables.Register</c> 里最先登记，也就是说它的译法会优先于各区域表。
/// 因此这里只放「确实有调用点」的词，避免一条没人用的通用译法遮蔽后面更贴切的区域译法。
/// </remarks>
internal static class ZhCommon
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        ("Cancel", "取消"),
        ("None", "无"),
        ("Unknown", "未知"),
        ("Open settings", "打开设置"),
        ("Support me", "支持作者"),
    ];
}
