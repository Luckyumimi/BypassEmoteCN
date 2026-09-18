namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>UI/CreateModWindow.cs</c>（把一对情感动作生成 Penumbra 模组的窗口）。 </summary>
/// <remarks> "Search emotes..." 已由 <see cref="ZhEmoteWindow"/> 与 <see cref="ZhEmotePoolTab"/> 提供，此处不再重复。 </remarks>
internal static class ZhCreateModWindow
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // ── 窗口标题 ──────────────────────────────────────────────────────────
        ("Bypass Emote - Create a mod", "Bypass Emote - 创建模组"),

        // ── 顶部说明 ──────────────────────────────────────────────────────────
        ("This window allows you to create a permanent swap mod, and it stays unaffected by BypassEmote. You own it, and you manage it, like any other mod.",
            "此窗口用于创建一个永久替换模组，它不受 Bypass Emote 影响；它归你所有，也和其他模组一样由你自行管理。"),

        // ── 表格左侧的字段名 ──────────────────────────────────────────────────
        ("Emote to play", "播放的情感动作"),
        ("Played over", "替换到"),
        ("Races covered", "适用种族"),
        ("Mod name", "模组名称"),
        ("Enable on creation", "创建时启用"),
        ("Highest priority", "最高优先级"),

        // ── 各字段的悬浮说明 ──────────────────────────────────────────────────
        ("The animation the mod plays. The one you have not unlocked.",
            "模组播放的动画，也就是你尚未解锁的那个。"),
        ("The emote you will actually use in game. The one you have unlocked.",
            "你在游戏中实际使用的那个情感动作，也就是你已解锁的那个。"),
        ("Which bodies the mod is written for.", "该模组为哪些种族生成。"),
        ("The name of the generated mod.", "生成的模组名称。"),
        ("Switches the mod on in your character's collection as soon as it exists.",
            "模组一创建就在你的角色集合中启用。"),
        ("Makes the mod have the highest priority. When off, it is created at priority 0.",
            "让该模组拥有最高优先级。关闭时以优先级 0 创建。"),

        // ── 选择框提示 ────────────────────────────────────────────────────────
        ("Pick the emote to play...", "选择要播放的情感动作..."),
        ("Pick the emote to play over...", "选择要替换到的情感动作..."),
        ("My emote mod", "我的情感动作模组"),
        ("Search races...", "搜索种族..."),
        ("No race covered", "尚未选择种族"),

        // ── 动画来源 ──────────────────────────────────────────────────────────
        ("Modded animation: {0}", "模组动画：{0}"),
        ("Vanilla animation", "原版动画"),

        // ── 匹配警告 ──────────────────────────────────────────────────────────
        ("You have not unlocked {0}.", "你还没有解锁 {0}。"),
        ("Your mod \"{0}\" already changes {1}.", "你的模组「{0}」已经修改了 {1}。"),

        // ── 种族范围警告 ──────────────────────────────────────────────────────
        ("{0} read the same animation file as {1}. {1}'s version plays for all of them.",
            "{0} 与 {1} 读取同一个动画文件，因此它们都会播放 {1} 的版本。"),
        ("This also changes the emote for {0}: they read a file the mod writes.",
            "这也会改变 {0} 的情感动作：它们会读取该模组写入的文件。"),

        // ── 创建按钮与前置条件 ────────────────────────────────────────────────
        ("Create", "创建"),
        ("An emote cannot be played over itself.", "情感动作不能替换到它自己身上。"),
        ("Pick at least one race.", "请至少选择一个种族。"),
        ("Pick both emotes and name the mod.", "请选择两个情感动作并为模组命名。"),

        // ── 结果与错误提示 ────────────────────────────────────────────────────
        ("Emote data is still loading. Try again in a moment.", "情感动作数据仍在加载中，请稍后再试。"),
        ("One of those emotes has no animation this can read.", "其中一个情感动作没有可读取的动画。"),
        ("Penumbra is not running.", "Penumbra 未运行。"),
    ];
}
