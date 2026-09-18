namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>UI/ConfigWindow.cs</c>（设置窗口）。 </summary>
internal static class ZhConfigWindow
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // ── 语言切换（由语言切换功能引入，勿改动结构） ─────────────────────────────
        ("Language", "语言"),
        ("Language used by this plugin's windows and chat messages.", "本插件窗口与聊天提示所使用的语言。"),
        ("Follow the game", "跟随游戏"),

        // ── 标签页 ────────────────────────────────────────────────────────────
        ("General settings", "通用设置"),
        ("Bypass Mode", "无视限制模式"),
        ("Emote overrides", "情感动作覆写"),

        // ── 下拉选项 ──────────────────────────────────────────────────────────
        ("When the emote ends", "情感动作结束时"),
        ("When you play the target emote", "播放目标动作时"),
        ("Never", "从不"),
        ("Multiple swaps", "多个替换"),
        ("One swap at a time", "仅保留一个替换"),
        ("Strict", "严格"),
        ("Lenient", "宽松"),
        ("Off", "关闭"),
        ("Very strict", "非常严格"),
        ("Allowed", "允许"),
        ("Last resort", "最后手段"),
        ("Blocked", "屏蔽"),
        ("Only when nothing else fits", "仅当无其他选择"),
        ("Allow", "允许"),
        ("Only when necessary", "仅在必要时"),
        ("On", "开启"),
        ("Same rank only", "仅同等级"),
        ("One rank below", "可低一级"),
        ("Anything allowed", "不限等级"),
        ("Emote Swap", "表情替换"),
        ("Direct Play", "直接播放"),

        // ── 设置行名称（Bypass Mode 页） ──────────────────────────────────────
        ("Mode", "模式"),
        ("Turn the swap off", "何时关闭替换"),
        ("Swaps at the same time", "同时替换数量"),
        ("Kept swaps per emote", "每个情感动作保留的替换"),
        ("Hide your name on the mod", "在模组中隐藏你的名字"),
        ("Always cache-break (experimental)", "始终刷新缓存（实验性）"),
        ("Loop matching", "循环匹配"),
        ("Turn matching", "转身匹配"),
        ("Sound matching", "音效匹配"),
        ("Spread swaps over several emotes", "将替换分散到多个情感动作"),
        ("Emotes used per behaviour", "每种行为可用的情感动作数"),
        ("How close a spread emote must fit", "分散替换所需的接近程度"),
        ("Repeat an error at most every", "同一错误最短重复间隔"),
        ("Repeat a warning at most every", "同一警告最短重复间隔"),
        ("Unsafe toggle", "不安全开关"),
        ("Emotes your mods change", "被模组修改的情感动作"),
        ("Loops on idle poses", "在待机动作上循环"),
        ("Show swap messages", "显示替换消息"),
        ("Show error messages", "显示错误消息"),
        ("Show warning messages", "显示警告消息"),
        ("Face your target automatically", "自动面向目标"),

        // ── 设置行名称（General settings 页） ─────────────────────────────────
        ("Enable the plugin", "启用插件"),
        ("Bypass emotes from locked hotbar slots", "无视热键栏格子解锁限制"),
        ("Show locked emotes in the game's Emote window", "在游戏情感动作窗口显示未解锁动作"),
        ("Do not grey out locked emotes", "未解锁的情感动作不变灰"),
        ("Stop companion emotes when they move", "宠物移动时停止其情感动作"),
        ("Show update notifications", "显示更新通知"),
        ("Show the changelog after an update", "更新后显示更新日志"),
        ("Show windows in GPose", "在 GPose 中显示窗口"),
        ("Show windows while the game UI is hidden", "游戏界面隐藏时仍显示窗口"),
        ("Check now", "立即检查"),
        ("Support me", "支持作者"),

        // ── 分节标题 ──────────────────────────────────────────────────────────
        ("Plugin", "插件"),
        ("Windows", "窗口"),
        ("Updates", "更新"),
        ("Safety", "安全"),
        ("Direct play", "直接播放"),
        ("Matching", "匹配"),
        ("Penumbra", "Penumbra"),
        ("Chat messages", "聊天消息"),

        // ── 长段落 / 提示 ─────────────────────────────────────────────────────
        ("Not all sync services support Direct Play.",
            "并非所有同步服务都支持直接播放。"),

        ("In safe mode you can only bypass emotes from the base pose (pose 0) of your current stance.",
            "安全模式下，你只能从当前姿态的基础姿势（姿势 0）无视限制播放情感动作。"),

        ("Safe mode is not 100% guaranteed to be safe either. It prevents Direct Play from being used in states where it was proved to "
         + "go wrong, but I can not prove the absence of issues. Use Emote Swap if you want to be 100% safe, it will behave almost the same way.",
            "安全模式也无法 100% 保证绝对安全。它能避免直接播放在已知会出问题的状态下使用，但我无法证明不存在其他问题。"
            + "想要 100% 安全就请使用表情替换，它的表现几乎完全一致。"),

        ("This is unsafe. Forcing an emote outside your base pose is, in theory, detectable by the server.",
            "这不安全。在基础姿势之外强制播放情感动作，理论上可被服务器检测到。"),

        ("In practice it is a non-issue. This has been a thing in other tools and plugins (and still is in some of them), "
         + "which people have used for years without trouble. Go back to safe mode, or even better, to emote swap, if you are uncomfy with this.",
            "实际上这基本不算问题。其他工具和插件一直都有类似做法（有些至今仍在使用），人们用了很多年也没出过事。"
            + "如果你对此感到不安，请回到安全模式，或者更好——回到表情替换。"),

        ("Lets Direct Play bypass an emote whatever pose you are in."
         + "\n\nLeave it off unless you know what you are doing: off, the plugin only plays emotes from states where "
         + "nothing can be noticed.",
            "让直接播放在任何姿势下都能无视限制播放情感动作。"
            + "\n\n除非你清楚自己在做什么，否则请保持关闭：关闭时插件只会在不会被察觉的状态下播放情感动作。"),

        ("Not all sync services support it. In safe mode you can only bypass emotes from the base pose (pose 0) of your current stance.",
            "并非所有同步服务都支持。安全模式下，你只能从当前姿态的基础姿势（姿势 0）无视限制播放情感动作。"),

        ("Not recommended. Not all sync services support it. Emote Swap is safer and works over any sync service.",
            "不推荐。并非所有同步服务都支持。表情替换更安全，且适用于任何同步服务。"),

        ("\"Emote Swap plays\" your emote over one your character owns, through a Penumbra mod. Other players see it "
         + "over any sync service."
         + "\n\"Direct Play\" sends the emote to the game itself, which not every sync service supports.",
            "「表情替换」通过 Penumbra 模组，把你想要的情感动作替换到角色已拥有的情感动作上播放。其他玩家在任何同步服务中都能看到。"
            + "\n「直接播放」把情感动作直接发送给游戏本体，并非所有同步服务都支持。"),

        // ── General settings 帮助文本 ─────────────────────────────────────────
        ("Lets you bypass emotes with the vanilla emote commands (/beesknees, /tea, and so on) and with hotbar slots."
         + "\nThe main window and the /be command work regardless of this setting.",
            "允许你使用游戏原生的情感动作指令（/beesknees、/tea 等）和热键栏格子来无视限制播放情感动作。"
            + "\n主窗口和 /be 指令不受此设置影响。"),

        ("Bypasses a locked emote when you press its hotbar slot."
         + "\nRight-click an emote in the main window to assign it to a slot.",
            "按下未解锁情感动作所在的热键栏格子时，无视限制直接播放它。"
            + "\n在主窗口中右键点击情感动作即可将其分配到热键栏格子。"),

        ("Lists the emotes you have not unlocked in the game's own emote window."
         + "\nClose and reopen the emote window for changes to appear.",
            "在游戏自带的情感动作窗口中列出你尚未解锁的情感动作。"
            + "\n关闭并重新打开该窗口后改动才会生效。"),

        ("Keeps the emotes you have not unlocked lit in the game's emote window and on your hotbars."
         + "\nPurely cosmetic.",
            "让未解锁的情感动作在游戏情感动作窗口和热键栏中保持亮起。"
            + "\n仅为显示效果。"),

        ("Stops your minion, pet or chocobo's looped emote when they move.",
            "当你的宠物、召唤物或陆行鸟移动时，停止其正在循环的情感动作。"),

        ("Keeps this plugin's windows visible while you are in GPose.",
            "进入 GPose 时保持本插件的窗口可见。"),

        ("Keeps this plugin's windows visible when you hide the game UI.",
            "隐藏游戏界面时保持本插件的窗口可见。"),

        ("Tells you in chat and on screen when a new version of the plugin has been installed.",
            "插件安装新版本时，会在聊天栏和屏幕上通知你。"),

        ("Opens the changelog window after an update.",
            "更新后自动打开更新日志窗口。"),

        // ── Bypass Mode 帮助文本 ──────────────────────────────────────────────
        ("Turns your character toward your target when you bypass an emote.",
            "无视限制播放情感动作时，让角色转向目标。"),

        ("Due to detectability, you need to be in the base pose (pose 0) of your current stance to bypass emotes "
         + "in safe mode.",
            "出于可被检测的考虑，安全模式下你需要处于当前姿态的基础姿势（姿势 0）才能无视限制播放情感动作。"),

        ("\"Strict\" only puts a looping emote on another looping one."
         + "\n\"Lenient\" lets a looping emote play once on a one time emote when no better match exists."
         + "\n\nRecommended: \"Strict\", or \"Lenient\" if you really don't have many emotes.",
            "「严格」只会把循环情感动作替换到另一个循环情感动作上。"
            + "\n「宽松」在没有更好选择时，允许循环情感动作在一次性情感动作上播放一次。"
            + "\n\n推荐：「严格」；如果你确实没多少情感动作，可以选「宽松」。"),

        ("Emotes have different turn behaviors when you target someone. Some emotes will make your torso turn (i.e: /hum), some only your head (i.e: /stepdance),"
         + "some will only make your eyes follow your target (i.e: /beesknees) while others will not move at all (i.e:/guard)."
         + "\n\n\"Very strict\" only picks emotes that behaves the same way."
         + "\n\"Strict\" allows eye following differences but keeps emotes head and body turn behaviors."
         + "\n\"Lenient\" allows any turn behavior."
         + "\n\nThe plugin will still always try to find the best match first, regardless of the selected rule."
         + "\n\nRecommended: \"Lenient\".",
            "情感动作在锁定目标时的转身表现各不相同。有些动作会让你的躯干转动（如 /hum），有些只转动头部（如 /stepdance），"
            + "有些只会让你的视线跟随目标（如 /beesknees），还有一些完全不会动（如 /guard）。"
            + "\n\n「非常严格」只选择表现完全一致的情感动作。"
            + "\n「严格」允许视线跟随上的差异，但要求头部和身体的转身表现一致。"
            + "\n「宽松」允许任何转身表现。"
            + "\n\n无论选择哪种规则，插件都仍然会优先尝试寻找最佳匹配。"
            + "\n\n推荐：「宽松」。"),

        ("\"Strict\" never puts an emote on one that makes sound."
         + "\n\"Lenient\" allows matching emotes that make sounds together."
         + "\n\"Off\" will let emotes play regardless of sound."
         + "\n\nThis is to prevent vanilla people from seeing you play fume which could annoy other vanilla players, for example."
         + "\n\nRecommended: \"Lenient\".",
            "「严格」绝不会把情感动作替换到会发出音效的动作上。"
            + "\n「宽松」允许把都会发出音效的情感动作互相匹配。"
            + "\n「关闭」不考虑音效，直接播放。"
            + "\n\n举例来说，这是为了避免原版玩家看到你在播放 fume，从而可能打扰到其他原版玩家。"
            + "\n\n推荐：「宽松」。"),

        ("Gives each bypassed emote a target emote of its own. This is useful when you want to bypass multiple emotes quickly."
         + "\n\n\"Off\" would make it so other people "
         + "on your sync service would see you redraw constantly."
         + "\n\"Only when necessary\" spreads nothing while the game plays fresh content on its own, and "
         + "steps in for the emotes that would show a stale frame once a game patch breaks that."
         + "\n\"On\" always spreads emotes."
         + "\n\nRecommended: \"On\" if you want other people on your sync service to always see you properly without "
         + "redrawing all the time, otherwise highly recommended to leave it on \"Only when necessary\" and not \"Off\".",
            "为每个被无视限制播放的情感动作分配各自的目标情感动作。想在短时间内连续播放多个情感动作时很有用。"
            + "\n\n「关闭」会让同步服务上的其他玩家看到你不断重新加载角色。"
            + "\n「仅在必要时」在游戏能自行播放新内容时不分散替换，而当游戏更新破坏该机制、某些动作会显示旧画面时再介入。"
            + "\n「开启」始终分散情感动作。"
            + "\n\n推荐：如果你希望同步服务上的其他玩家不必频繁重新加载也能始终看到正确的效果，请选「开启」；"
            + "否则强烈建议保持「仅在必要时」，不要选「关闭」。"),

        ("How many different target emotes one kind of emote may swap to.",
            "同一种情感动作可以替换到多少个不同的目标情感动作上。"),

        ("Takes effect when \"Spread swaps over several emote\" is enabled. This determines which emotes become available for a source emote. "
         + "Basically, if you want to spread swaps over 5 emotes, and you try to bypass an emote but you only have 2 same-rank targets available, "
         + "this is how it will determine what to do in this scenario. A rank is basically a category of emotes with similar characteristics (same turn behaviour, etc)."
         + "\n\n\"Same rank only\" strictly picks targets of the same rank."
         + "\n\"One rank below\" also accepts targets one rank below."
         + "\n\"Anything allowed\" picks any target it can find, regardless of the rank."
         + "\n\nNone of them ever breaks your other rules."
         + "\n\nRecommended: \"One rank below\", or \"Same rank only\" if you want behavior accuracy.",
            "在启用「将替换分散到多个情感动作」时生效，决定某个源情感动作可以使用哪些情感动作作为目标。"
            + "简单来说，如果你想分散到 5 个情感动作，但只有 2 个同等级目标可用时，就由它来决定如何处理。"
            + "等级大致就是具有相同特征（转身表现等）的一类情感动作。"
            + "\n\n「仅同等级」只选择同等级的目标。"
            + "\n「可低一级」也接受低一个等级的目标。"
            + "\n「不限等级」会选择任何能找到的目标，不考虑等级。"
            + "\n\n以上选项都不会破坏你的其他规则。"
            + "\n\n推荐：「可低一级」；如果你更看重表现准确，可以选「仅同等级」。"),

        ("Determines whether to block unlocked emotes from being picked when they are modified by at least one of your mods. "
         + "This prevents other people from seeing other modded emotes you might have before the swap takes place."
         + "\nAs an example, you have a mod on beesknees, and you try to bypass /conduct which happens to land on beesknees: "
         + "other players might or might not see the modded beesknees for a moment."
         + "\n\n\"Allowed\" allows using unlocked emotes that are modified by one of your mods."
         + "\n\"Last resort\" uses one only when nothing else fits."
         + "\n\"Blocked\" never uses one, and show an error message in the chat with options to disable or open the mod in penumbra."
         + "\n\nRecommended: \"Last resort\", or \"Blocked\" if you absolutely don't want your modded emotes to accidentaly be seen.",
            "决定当已解锁的情感动作被你的某个模组修改时，是否禁止将其作为目标。这可以避免其他玩家在替换生效前看到你拥有的其他模组效果。"
            + "\n例如：你给 beesknees 装了模组，而你想无视限制播放 /conduct，它正好匹配到 beesknees：其他玩家可能会在瞬间看到被模组修改后的 beesknees。"
            + "\n\n「允许」允许使用被你的模组修改过的已解锁情感动作。"
            + "\n「最后手段」仅在没有其他合适选项时才使用。"
            + "\n「禁止」绝不使用，并在聊天栏显示错误提示，附带禁用该模组或在 Penumbra 中打开模组的选项。"
            + "\n\n推荐：「最后手段」；如果你绝对不希望自己的模组效果被意外看到，可以选「禁止」。"),

        ("When no unlocked looped emote fits as a target, your current idle pose may be eligible instead. "
         + "The emote you try to bypass will then be targeted onto your current idle pose. This will cause a redraw of your character when triggered."
         + "\n\n\"Never\" blocks idle poses from being used as targets."
         + "\n\"Only when nothing else fits\" uses the pose only when literally no unlocked looped emote could have played here at all. "
         + "This will not use your idle pose if any other emote would have been available if it wasn't blocked."
         + "\n\"Allow\" always falls back to your idle pose when no other options are available."
         + "\n\nRecommended: \"Only when nothing else fits\".",
            "当没有已解锁的循环情感动作可以作为目标时，你的当前待机动作也可以作为候选。此时你要无视限制播放的情感动作会被替换到当前待机动作上。"
            + "触发时会导致角色重新加载。"
            + "\n\n「从不」禁止将待机动作作为目标。"
            + "\n「仅当无其他选择」只在确实没有任何已解锁的循环情感动作可播放时才使用待机动作。"
            + "如果还有其他情感动作（只是被规则挡住）可用，就不会改用待机动作。"
            + "\n「允许」在没有其他可用选项时总是回退到你的待机动作。"
            + "\n\n推荐：「仅当无其他选择」。"),

        ("\"When the emote ends\" puts your real emote back as soon as the animation stops."
         + "\n\"When you play the target emote\" keeps the swap enabled until the next time you play the target emote."
         + "\n\"Never\" keeps the swap live until another swap claims the same target emote."
         + "\n\nRecommended: \"When you play the target emote\". \"When the emote ends\" is not recommended, as people will "
         + "see you redraw constantly after swapping.",
            "「情感动作结束时」在动画停止后立刻恢复你原本的情感动作。"
            + "\n「播放目标动作时」保持替换生效，直到你下次播放该目标情感动作。"
            + "\n「从不」让替换一直保留，直到另一个替换占用了同一个目标情感动作。"
            + "\n\n推荐：「播放目标动作时」。「情感动作结束时」不推荐，因为替换后其他人会看到你不断重新加载。"),

        ("\"Multiple swaps\" keeps multiple swaps active at the same time in the mod."
         + "\n\"One swap at a time\" turns the previous swaps off as soon as a new one starts."
         + "\n\nRecommended: \"Multiple swaps\", unless for some reason you want to only keep one swap at a time.",
            "「多个替换」让模组中同时保留多个替换。"
            + "\n「仅保留一个替换」在新替换开始时立即关闭之前的替换。"
            + "\n\n推荐：「多个替换」，除非你出于某种原因只想同时保留一个替换。"),

        ("How many swaps are kept per target emote. 0 keeps them all."
         + "\n\nEach kept swap stays as an option of the generated Penumbra mod, so playing an emote "
         + "again enables it again without rebuilding it. The only cost is that the mod gets bigger.",
            "每个目标情感动作保留多少个替换。设为 0 表示全部保留。"
            + "\n\n每个保留的替换都会作为生成的 Penumbra 模组的一个选项存在，因此再次播放该情感动作时会直接启用，而无需重新生成。"
            + "唯一的代价是模组会变大。"),

        ("Names the generated mod after your initials instead of your full name and world.",
            "生成的模组使用你的姓名首字母命名，而不是完整的角色名和服务器名。"),

        ("Applies the cache-break mechanism to every emote, even those you own. This is a bonus feature and "
         + "is experimental. This will allow the client to always \"refresh\" animations so you never have "
         + "to redraw yourself or stop emoting to apply an animation change.",
            "对所有情感动作（包括你自己已拥有的）应用缓存刷新机制。这是一个附加的实验性功能。"
            + "它能让客户端始终「刷新」动画，因此你无需重新加载角色或停止情感动作，就能应用动画变化。"),

        ("Shows a message in chat when an emote is swapped.",
            "发生替换时在聊天栏显示消息。"),

        ("Shows an error in chat when a swap could not be done.",
            "替换失败时在聊天栏显示错误。"),

        ("How long the same error stays quiet after you have seen it."
         + "\n\nAccepts time format: 5m, 300s, 5m10s, 1h."
         + "\nSet to 0 to see every one of them.",
            "同一条错误在你看到之后会静默多久。"
            + "\n\n支持时间格式：5m、300s、5m10s、1h。"
            + "\n设为 0 表示每条都显示。"),

        ("Shows a warning in chat when a swap happened, but not the way you would expect.",
            "替换发生但结果与预期不符时，在聊天栏显示警告。"),

        ("How long the same warning stays quiet after you have seen it."
         + "\n\nAccepts time format: 5m, 300s, 5m10s, 1h."
         + "\nSet to 0 to disable.",
            "同一条警告在你看到之后会静默多久。"
            + "\n\n支持时间格式：5m、300s、5m10s、1h。"
            + "\n设为 0 表示不再提示。"),

        // ── 告警与状态 ────────────────────────────────────────────────────────
        ("This game build is not approved yet, this will not work.",
            "当前游戏版本尚未通过审批，此功能不会生效。"),

        ("Not running: {0}.", "未运行：{0}。"),

        ("Leave this on, otherwise bypassing several animations in a row may look broken.",
            "请保持开启，否则连续播放多个动画时可能看起来出问题。"),

        // ── 补丁审批 ──────────────────────────────────────────────────────────
        ("Bypass Emote has not been approved for this game build.",
            "Bypass Emote 尚未针对当前游戏版本通过审批。"),

        ("Emote swaps may behave oddly until the build is approved.\nThe plugin will automatically fetch updates every 10 minutes to check if it was approved.",
            "在版本通过审批之前，表情替换可能表现异常。\n插件每 10 分钟会自动获取一次更新，以检查是否已通过审批。"),

        ("The Bypass Emote you are using is not the original plugin.", "你所在使用的Bypass Emote不是原版插件"),

        ("This Chinese localization and CN client adaptation branch is maintained by luckyumimi. If you have any problem, please report the bug at https://github.com/Luckyumimi/BypassEmoteCN",
            "此汉化＆国服适配分支由luckyumimi负责，如果有任何问题，请前往https://github.com/Luckyumimi/BypassEmoteCN报告bug"),

        ("Open the project page", "打开项目主页"),

        ("not yet", "尚未"),

        ("Checked at {0}.", "检查时间：{0}。"),

        ("Enable Force Approval", "开启强制审批"),
        ("Disable Force Approval", "关闭强制审批"),
        ("Force approval is on.", "强制审批已开启。"),

        // ── 确认弹窗 ──────────────────────────────────────────────────────────
        ("Switch to Direct Play?", "切换到直接播放？"),
        ("Switch to Direct Play", "切换到直接播放"),
        ("Enable unsafe mode?", "开启不安全模式？"),
        ("Enable unsafe mode", "开启不安全模式"),
        ("Cancel", "取消"),
    ];
}
