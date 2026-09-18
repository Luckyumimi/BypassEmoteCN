namespace BypassEmote.Localization;

/// <summary> 中文文案：<c>Changelog/</c>（各版本更新说明）。 </summary>
internal static class ZhChangelog
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // ── 通用分节标题与重复文案 ─────────────────────────────────────────────
        ("New Features", "新功能"),
        ("Bug fixes", "错误修复"),
        ("Bug Fixes", "错误修复"),
        ("Technical Changes", "技术改动"),
        ("Various technical enhancements.", "多项技术性改进。"),
        ("Various bug fixes over the versions.", "修复了各版本中的多项错误。"),
        ("Emote handling enhancement", "情感动作处理改进"),
        ("Improved Emote Detection", "情感动作检测改进"),

        // ── 1.4.0.0 ───────────────────────────────────────────────────────────
        ("Various QoLs and Changelog System", "多项易用性改进与更新日志系统"),
        ("Major update introducing a comprehensive changelog system with UI integration, configuration tracking, and enhanced user experience features.",
            "一次重大更新，引入了完整的更新日志系统，包含界面集成、配置记录以及多项体验优化。"),
        ("Changelog system", "更新日志系统"),
        ("Introduced ChangelogWindow UI for viewing updates", "新增更新日志窗口界面，用于查看更新内容"),
        ("Accessible via main plugin window book button", "可通过主窗口的书本按钮打开"),
        ("Clean, organized display of version history", "清晰、有条理地展示版本历史"),
        ("Interactive elements and color-coded entries", "支持交互元素与按颜色区分的条目"),
        ("Comprehensive changelog management system", "完整的更新日志管理系统"),
        ("Versioned changelog entries with structured data", "使用结构化数据的版本化更新日志条目"),
        ("Automatic changelog version tracking", "自动记录更新日志版本"),
        ("Support for rich formatting and icons", "支持富文本格式与图标"),
        ("Added new emote data collected from FFXIVCollect", "新增从 FFXIVCollect 收集的情感动作数据"),
        ("Added which patch the emote is from", "新增该情感动作所属的游戏版本"),
        ("Added the obtention methods to get the emote", "新增获取该情感动作的方式"),
        ("Configuration & Settings", "配置与设置"),
        ("Updated configuration system to track changelog versions", "更新配置系统以记录更新日志版本"),
        ("Show changelog on update", "更新后显示更新日志"),
        ("Last seen changelog version tracking", "记录最后查看过的更新日志版本"),
        ("Added an option to show update notifications", "新增显示更新通知的选项"),
        ("User Interface", "用户界面"),
        ("Enhanced main plugin window with new action buttons in the title bar", "增强主窗口，在标题栏新增操作按钮"),
        ("Added changelog button in title bar", "在标题栏新增更新日志按钮"),
        ("New settings access button", "新增设置入口按钮"),
        ("Integrated support button (Ko-Fi)", "集成赞助按钮（Ko-Fi）"),
        ("Donate", "捐赠"),
        ("Added favorited emotes feature", "新增情感动作收藏功能"),
        ("Easily access frequently used emotes", "快速访问常用情感动作"),
        ("Mark/unmark emotes as favorites", "将情感动作标记/取消标记为收藏"),
        ("View all favorited emotes in the \"Fav\" tab", "在「收藏」标签页查看所有收藏的情感动作"),
        ("Added an info circle you can hover to see what patch the emote is from, and how to obtain it",
            "新增信息图标，悬停即可查看该情感动作来自哪个游戏版本以及如何获取"),

        // ── 1.4.1.0 ───────────────────────────────────────────────────────────
        ("Fixed an incorrect detection of game emotes.", "修复了游戏情感动作识别错误的问题。"),
        ("The game will now display Patch 7.35 game emotes.", "游戏现在会显示 7.35 版本的情感动作。"),

        // ── 1.5.0.0 ───────────────────────────────────────────────────────────
        ("Bugfix, emote handling enhancement, and internal system change", "错误修复、情感动作处理改进与内部系统调整"),
        ("Fixed emote playing capabilities, now allowing to bypass emotes when sitting or on a mount. Integrated NoireLib for improved utilities and features.",
            "修复了情感动作的播放能力，现在坐着或骑乘时也能无视限制播放情感动作。集成 NoireLib 以改进工具与功能。"),
        ("You can now emote while sitting, groundsitting, mounted or riding pillion.",
            "现在你可以坐着、坐在地上、骑乘或坐在他人坐骑后座时播放情感动作。"),
        ("You can now show emote IDs in the emote list.", "现在可以在情感动作列表中显示情感动作 ID。"),
        ("Fixed a bug where Dalamud UI language would be used to get emote commands instead of game language when typing auto-translate emote commands.",
            "修复了在输入自动翻译情感动作指令时，使用 Dalamud 界面语言而非游戏语言来获取情感动作指令的问题。"),
        ("Fixed a bug where NPCs emotes would bypass once and then block afterwards.",
            "修复了 NPC 的情感动作只能无视限制播放一次、之后就会被拦截的问题。"),
        ("Integrated NoireLib for improved utilities and changelog system.", "集成 NoireLib，以改进工具与更新日志系统。"),
        ("Refactored emote and character helpers to use NoireLib utilities.", "重构情感动作与角色辅助代码，改用 NoireLib 工具。"),

        // ── 1.5.1.0 ───────────────────────────────────────────────────────────
        ("Bugfix, new UI option and various internal changes", "错误修复、新增界面选项与多项内部调整"),
        ("For debug purposes, you can now show absolutely *all* in game emotes, including invalid ones, with an option in the emote window. Look for \"Show Invalid Emotes\".",
            "为了便于调试，现在可以通过情感动作窗口中的选项显示游戏中*所有*情感动作，包括无效的那些。请查找「显示无效情感动作」。"),
        ("Emotes previously in the \"Other\" tab had disappeared. Those are now back.",
            "之前位于「其他」标签页的情感动作消失了，现在已恢复。"),
        ("Other various bug fixes.", "其他多项错误修复。"),
        ("Refactored the code to use the new NoireLib helpers and utilities.", "重构代码以使用新的 NoireLib 辅助与工具。"),

        // ── 1.5.1.1 ───────────────────────────────────────────────────────────
        ("Hotfix", "紧急修复"),
        ("Bug Hotfix", "错误紧急修复"),
        ("Fixed the changelog that would not show anymore.", "修复了更新日志不再显示的问题。"),
        ("Fixed the settings to show the changelog and print in chat on update that were not enforced anymore.",
            "修复了「更新后显示更新日志」和「更新后在聊天栏提示」这两个设置不再生效的问题。"),

        // ── 1.6.0.0 ───────────────────────────────────────────────────────────
        ("Enhances the emote handling both locally and while synced.", "改进本地与同步状态下的情感动作处理。"),
        ("Added support for special emotes such as /dote and /allsaintscharm where the target would not be handled properly.",
            "新增对特殊情感动作的支持，例如 /dote 和 /allsaintscharm，这些动作的目标此前无法被正确处理。"),
        ("Added /be sync and /be syncall which will respectively sync the emotes of bypassed players only, or all players on the map.",
            "新增 /be sync 与 /be syncall：前者只同步无视限制播放情感动作的玩家，后者同步地图上的所有玩家。"),
        ("Fixed emote not being bypassed when you passed the emote command with parameters, such as \"/beesknees motion\".",
            "修复了在情感动作指令后带参数（例如 \"/beesknees motion\"）时无法无视限制播放的问题。"),
        ("Fixed a bug where bypassing a looped emote would not cancel after mounting-dismounting, interacting with NPCs, casting, etc.",
            "修复了无视限制播放循环情感动作后，在上下坐骑、与 NPC 交互、咏唱等情况下不会取消的问题。"),
        ("Fixed a bug where you could bypass emotes in GPose.", "修复了在 GPose 中可以无视限制播放情感动作的问题。"),
        ("Improved how the syncing works, and specifically how other players are handled when they stop emoting.",
            "改进了同步机制，尤其是其他玩家停止播放情感动作时的处理方式。"),

        // ── 1.6.1.2 ───────────────────────────────────────────────────────────
        ("Various bug fixes", "多项错误修复"),
        ("Fixes a few bugs with some specific emotes and syncing.", "修复了若干与特定情感动作和同步相关的错误。"),
        ("Fixed some emotes not being bypassed properly, for example /waterfloat and /waterflip.",
            "修复了部分情感动作无法正确无视限制播放的问题，例如 /waterfloat 和 /waterflip。"),
        ("Improved how the syncing works (again), and specifically how other players are handled when they stop emoting.\n"
         + "A small delay will be added before the emote is sent over IPC.\nFurthermore, movement detections for other players has been improved to "
         + "add some leeway so that the emote doesnt get stopped when the player is still moving a bit.",
            "再次改进了同步机制，尤其是其他玩家停止播放情感动作时的处理方式。\n"
            + "情感动作通过 IPC 发送前会加入一小段延迟。\n此外，其他玩家的移动检测也加入了容差，因此玩家仍在轻微移动时情感动作不会被中断。"),
        ("Fixed emotes bypassing under some conditions that are not valid (Casting, in cutscene, in GPose, In event ...).",
            "修复了在某些本不应生效的情况下仍然无视限制播放的问题（咏唱中、过场动画中、GPose 中、事件中……）。"),

        // ── 1.6.2.0 ───────────────────────────────────────────────────────────
        ("Facial expressions", "面部表情"),
        ("Fixed a few bugs concerning facial expressions and facing targets.", "修复了若干与面部表情和面向目标相关的错误。"),
        ("Prevented facial expressions from stopping bypassed looped emotes.", "避免面部表情中断已无视限制播放的循环情感动作。"),
        ("Facial expression emotes no longer makes the character face its target.", "面部表情类情感动作不再让角色转向目标。"),
        ("Additionnaly, the character will no longer rotate when emoting to itself (self-targetting).",
            "此外，角色对自己播放情感动作（自我锁定）时不再转向。"),
        ("Commands /be sync and /be syncall will now better handle syncing emotes that have intro + loop phases, "
         + "hence improving the \"syncing\", where a slight delay would occur before for those type of emotes.",
            "/be sync 与 /be syncall 现在能更好地同步带前奏 + 循环阶段的情感动作，"
            + "从而改善这类情感动作此前会出现轻微延迟的「同步」表现。"),

        // ── 1.6.6.0 ───────────────────────────────────────────────────────────
        ("Fixed a bug with facing targets.", "修复了面向目标相关的错误。"),
        ("Minions, Pets, Chocobos & Battle NPCs support", "宠物、召唤兽、陆行鸟与战斗 NPC 支持"),
        ("Added support for Minions, Pets, Chocobos & Battle NPCs. Revised IPC.",
            "新增对宠物、召唤兽、陆行鸟与战斗 NPC 的支持。重做了 IPC。"),
        ("Added support for Minions, Pets and Chocobos. You can now apply any emote to your own minions, pets and chocobos provided they have been summoned and turned human.",
            "新增对宠物、召唤兽与陆行鸟的支持。只要它们已被召唤并变为人类形态，你现在就可以对它们播放任何情感动作。"),
        ("Added support for Battle NPCs. You can now apply any emote Battle NPCs in the world.",
            "新增对战斗 NPC 的支持。你现在可以对世界中的战斗 NPC 播放任何情感动作。"),
        ("Revised IPC to support above changes.", "重做 IPC 以支持上述改动。"),
        ("Added the /bem command to apply emotes to your minion, provided it's been summoned and turned human.\n"
         + "You do not need to target the minion, and just like /bet, you can use an emote argument or the stop argument (/bem stop, /bem beesknees, /bem /tea, ...).\n"
         + "Useful for untargettable minions such as the cushion or the campfire.",
            "新增 /bem 指令，用于对你的宠物播放情感动作（前提是它已被召唤并变为人类形态）。\n"
            + "无需锁定该宠物；与 /bet 一样，可以使用情感动作参数或停止参数（/bem stop、/bem beesknees、/bem /tea 等）。\n"
            + "对于坐垫、篝火等无法锁定的宠物很有用。"),
        ("Additionally, added the /bep command to apply emotes to your pet (carbuncle and eos), provided it's been summoned and turned human.",
            "此外，新增 /bep 指令，用于对你的召唤兽（carbuncle 和 eos）播放情感动作（前提是它已被召唤并变为人类形态）。"),
        ("Additionally, added the /bec command to apply emotes to your chocobo, provided it's been summoned and turned human.",
            "此外，新增 /bec 指令，用于对你的陆行鸟播放情感动作（前提是它已被召唤并变为人类形态）。"),
        ("Added the possibility to apply emotes to your owned entities from the emote selector window (/be) via right click.",
            "新增通过右键点击，在情感动作选择窗口（/be）中对自己拥有的单位播放情感动作的功能。"),
        ("Added a configuration option to choose whether you want your minion, pet or chocobo to stop emoting when it moves.\n"
         + "Useful for the Plush Cushion or the Campfire if you're using SimpleHeels' temporary offsets, or for non-static entities.",
            "新增配置选项，可选择宠物、召唤兽或陆行鸟移动时是否停止播放情感动作。\n"
            + "如果你在使用 SimpleHeels 的临时偏移，或者面对非静态单位，这对毛绒坐垫或篝火很有用。"),

        // ── 1.7.2.0 ───────────────────────────────────────────────────────────
        ("Enhances the target handling and the emote bypassing.", "改进目标处理与情感动作的无视限制播放。"),
        ("BypassEmote now handles hotbar emote slots. If you click a locked emote hotbar slot (greyed out), the emote will play.",
            "BypassEmote 现在可以处理热键栏上的情感动作格子。点击未解锁情感动作的格子（灰色显示）时，该情感动作就会播放。"),
        ("The soft target is now properly handled. If you have both a target and a soft target, bypassing an emote will prioritize the soft target.",
            "现在可以正确处理软目标。若同时存在目标与软目标，无视限制播放情感动作时会优先使用软目标。"),
        ("Reworked the IPC Data being sent to consumers.", "重做了发送给使用方的 IPC 数据。"),

        // ── 1.7.3.1 ───────────────────────────────────────────────────────────
        ("Hotbar integration enhancement", "热键栏集成改进"),
        ("Allows to add emotes to hotbars.", "允许将情感动作添加到热键栏。"),
        ("Added the possibility to assign emotes to hotbar slots.\nRight click an emote in the main UI, and an option will appear if the emote is assignable.",
            "新增将情感动作分配到热键栏格子的功能。\n在主窗口中右键点击情感动作，若该情感动作可分配，就会出现相应选项。"),
        ("Added a new config option for enabling/disabling bypassing emotes on emote hotbar slot click.",
            "新增配置选项，用于启用/禁用点击情感动作热键栏格子时的无视限制播放。"),

        // ── 2.0.0.0 ───────────────────────────────────────────────────────────
        ("Emote Swap", "表情替换"),
        ("Adds a new way of bypassing emotes through Penumbra, visible on every sync service.",
            "新增一种通过 Penumbra 无视限制播放情感动作的方式，在所有同步服务中均可见。"),
        ("Added Emote Swap, a new bypass mode, which is now the default one.\n"
         + "Instead of applying the animation on your character, the plugin looks for an emote you have unlocked that behaves like the one you asked for, "
         + "plays that one for real, and swaps its animation with the locked one through a Penumbra mod it manages for you.\n"
         + "The game itself plays the emote, so other players see it over any sync service without their plugin having to integrate BypassEmote.",
            "新增表情替换：这是一种新的无视限制模式，现已成为默认模式。\n"
            + "插件不再直接把动画应用到你的角色上，而是寻找一个你已解锁、且表现与你请求的情感动作相似的情感动作，真正播放它，"
            + "并通过插件为你管理的 Penumbra 模组把它与未解锁的情感动作的动画互换。\n"
            + "由于是游戏本身在播放该情感动作，其他玩家在任何同步服务中都能看到，无需他们的插件集成 BypassEmote。"),
        ("Emote Swap needs Penumbra installed and enabled.", "表情替换需要安装并启用 Penumbra。"),
        ("A prompt will appear on your first launch to let you pick the mode you want. You can change your mind at any time in the configuration window.",
            "首次启动时会弹出提示，让你选择想要的模式。之后随时可以在配置窗口中更改。"),
        ("Added a lot of configuration options for Emote Swap, such as how close a match has to be, when the swap is turned off, how many swaps are kept per emote, "
         + "and whether your name appears on the generated mod.",
            "为表情替换新增了大量配置选项，例如匹配需要多接近、何时关闭替换、每个情感动作保留多少个替换，"
            + "以及你的名字是否出现在生成的模组中。"),
        ("The default configuration is already set up the way it works best, you do not need to touch any of it.",
            "默认配置已经调整为效果最佳的状态，你无需改动任何设置。"),
        ("Every swap is kept as an option of the generated mod, so playing the same emote again puts it back without rebuilding anything.",
            "每个替换都会作为生成的模组的选项保留，因此再次播放同一情感动作时会直接恢复，无需重新构建。"),
        ("Emotes are now played according to your character's condition.\n"
         + "Sitting on the ground, sitting in a chair, mounted, riding pillion, swimming, diving, holding an umbrella or a torch, wearing a fashion accessory: "
         + "the plugin plays the variant that matches the state you are in, instead of forcing the standing animation on you.\n"
         + "If an emote cannot be played in the state you are in, the plugin will tell you so instead of playing something wrong.",
            "现在情感动作会根据角色所处的状态播放。\n"
            + "坐在地上、坐在椅子上、骑乘、坐在他人坐骑后座、游泳、潜水、手持雨伞或火把、佩戴时尚配饰："
            + "插件会播放与你当前状态相符的变体，而不是强行给你播放站立动画。\n"
            + "若某个情感动作无法在你当前的状态下播放，插件会明确告知，而不是播放错误的内容。"),
        ("Emote Swap follows the same rule, and will only play an emote you own that can be executed in the state you are in.",
            "表情替换遵循同样的规则，只会播放你已拥有、且能在当前状态下执行的情感动作。"),
        ("The main UI now shows the same condition icons as the game's own emote window, so you can see at a glance where an emote can be played.",
            "主界面现在会显示与游戏自带情感动作窗口相同的状态图标，让你一眼看出某个情感动作可以在哪些状态下播放。"),
        ("Added a safe mode to Direct Play, the original method, which is on by default.\n"
         + "While it is on, you can only bypass emotes from the base pose (pose 0) of your current stance. This is because the game client sends duplicate "
         + "change pose packets when your character is in any other pose, which is not something the plugin causes, but not something it can prevent either.\n"
         + "You can lift that limit in the configuration window if you know what you are doing.",
            "为直接播放（原有方式）新增了安全模式，默认开启。\n"
            + "开启时，你只能从当前姿态的基础姿势（姿势 0）无视限制播放情感动作。这是因为当角色处于其他姿势时，"
            + "游戏客户端会重复发送姿势变更封包——这并非插件造成的，但插件也无法阻止。\n"
            + "如果你清楚自己在做什么，可以在配置窗口中解除该限制。"),
        ("Safe mode is not a promise either, it only keeps Direct Play to the states where nothing has ever been seen to go wrong. Use Emote Swap if you want to be certain.",
            "安全模式同样不是保证，它只是把直接播放限制在从未出现过问题的状态。如果你想要万无一失，请使用表情替换。"),
        ("Added the possibility to create a permanent Penumbra mod from any emote.\n"
         + "Click the \"Create a mod\" button in the main UI, or right click an emote and pick \"Create a mod from this emote...\".\n"
         + "Useful if you want to keep an animation over one of your emotes without the plugin having to do anything.",
            "新增从任意情感动作创建永久 Penumbra 模组的功能。\n"
            + "点击主界面中的「创建模组」按钮，或右键点击某个情感动作并选择「用此情感动作创建模组...」。\n"
            + "如果你想在某个情感动作上保留一段动画、而又不需要插件做任何事，这会很有用。"),
        ("You can now stop Emote Swap from playing one of your own emotes, by clicking the ban icon next to its star.\n"
         + "Blocked emotes have their own tab in the main UI. Useful if you would rather not have a specific one of yours played, for example one that makes noise.",
            "现在你可以点击星标旁的禁止图标，阻止表情替换播放你自己的某个情感动作。\n"
            + "被屏蔽的情感动作在主界面中有独立标签页。如果你不希望自己的某个情感动作被拿来播放（例如会发出声音的那种），这会很有用。"),
        ("Added drag and drop from the main UI onto your hotbars. Drag an emote onto a slot of hotbars 1 to 10 and it will be assigned to it.",
            "新增从主界面拖放到热键栏的功能。将情感动作拖到热键栏 1 至 10 的任意格子上，即可将其分配到该格子。"),
        ("Added configuration options for the messages the plugin prints in chat, and for how often the same one is repeated.",
            "新增配置选项，用于控制插件在聊天栏输出哪些消息，以及同一条消息的重复间隔。"),
        ("Fixed emotes that make your character draw its weapon.", "修复了会让角色拔出武器的情感动作。"),
        ("Fixed visual effects not playing on some bypassed emotes.", "修复了部分被无视限制播放的情感动作不显示视觉效果的问题。"),
        ("Added Penumbra V4 support.", "新增 Penumbra V4 支持。"),
        ("BypassEmote now checks that the game patch you are on has been approved before enabling anything it finds by signature.\n"
         + "If a patch breaks something, the plugin turns those parts off by itself and checks every 10 minutes until it is approved again, "
         + "instead of doing something it should not.",
            "BypassEmote 现在会先确认你所在的游戏版本已通过审批，再启用任何通过特征码找到的功能。\n"
            + "如果某个游戏版本导致功能失效，插件会自动关闭这些部分，并每 10 分钟检查一次，直到重新通过审批，"
            + "而不是做出不该做的事。"),
        ("Moved a large part of the plugin's internals to NoireLib.", "将插件内部的大部分实现迁移至 NoireLib。"),

        // ── 2.1.0.0 ───────────────────────────────────────────────────────────
        ("Animation cache breaking", "动画缓存刷新"),
        ("Added \"Always cache-break\" option for Emote Swap, and various bug fixes.",
            "为表情替换新增「始终刷新缓存」选项，并修复多项错误。"),
        ("Added \"Always cache-break\", a new option for Emote Swap.\n"
         + "With it on, every emote you play will be cache-broken, especially the ones you own. Play an emote, enable or disable any mod affecting that emote, "
         + "play the emote again, the new animation refreshes and shows without needing to redraw nor to stop the emote.",
            "新增「始终刷新缓存」，这是表情替换的一个新选项。\n"
            + "开启后，你播放的每个情感动作都会刷新缓存，尤其是你自己已拥有的那些。播放一个情感动作，启用或禁用任何影响该动作的模组，"
            + "再播放一次，新的动画就会刷新并显示出来，无需重新加载角色，也无需停止情感动作。"),
        ("It is off by default, you can enable it in the configuration window.", "该选项默认关闭，可在配置窗口中开启。"),
        ("Added /belogs (also available as /be logs), which exports a zip of the plugin logs and settings to send to "
         + "the developer. Check its content before sending it, it is not meant to be posted publicly.",
            "新增 /belogs（也可写作 /be logs），用于将插件日志与设置的压缩包导出，以便发送给开发者。"
            + "发送前请检查其中的内容，它不适合公开发布。"),
        ("The plugin now recognizes the Korean and Chinese game clients.", "插件现在可以识别韩文与中文游戏客户端。"),
        ("Playing the real emote of a swap while the swap is still playing now refreshes the animation.\n"
         + "Before, playing for example /beesknees while a swap was using it kept showing the swapped animation "
         + "until you stopped emoting.",
            "在替换仍然生效时播放被替换的真实情感动作，现在会刷新动画。\n"
            + "在此之前，例如某个替换占用了 /beesknees，此时播放它仍会一直显示被替换后的动画，"
            + "直到你停止播放情感动作。"),
        ("Disabling the plugin now disables the generated mod.", "禁用插件时现在会一并禁用生成的模组。"),
        ("Playing an emote while a swapped idle pose is active now stops the idle pose swap.",
            "在被替换的待机动作生效期间播放情感动作，现在会停止该待机动作的替换。"),
        ("Reworked how animations are loaded around the game's caches.", "重做了围绕游戏缓存加载动画的方式。"),

        // ── 2.2.0.0 ───────────────────────────────────────────────────────────
        ("Emote overrides", "情感动作覆写"),
        ("Adds \"Emote overrides\", and various bug fixes and improvements.",
            "新增「情感动作覆写」，并带来多项错误修复与改进。"),
        ("Added the \"Emote overrides\" tab to the configuration window.\n"
         + "Pick a locked emote, then pick the emotes it is allowed to land on. Emote Swap will then use "
         + "your list instead of finding a best match, in the order you put it in.\n"
         + "If none of them can be played, the plugin tells you which ones it skipped and why.",
            "在配置窗口中新增「情感动作覆写」标签页。\n"
            + "先选择一个未解锁的情感动作，再选择允许它替换到的情感动作。之后表情替换会按你排列的顺序使用这份列表，"
            + "而不是自行寻找最佳匹配。\n"
            + "如果这些动作都无法播放，插件会告诉你跳过了哪些以及原因。"),
        ("The \"Fav\" and \"Blocked\" tabs of the main window now have their own dropdown, to add an emote to either list without hunting for it in the other tabs.",
            "主窗口的「收藏」与「屏蔽」标签页现在都有各自的下拉菜单，无需再到其他标签页里翻找，即可将情感动作加入任一列表。"),
        ("The \"Create a mod\" window now warns you about mismatches and other potentially unwanted behaviors, such as when you have not unlocked the target emote, when one of your own mods already changes it, "
         + "or when the two emotes have turn, loop, sound or intro mismatches.",
            "「创建模组」窗口现在会就不匹配以及其他可能不希望出现的情况给出警告，例如你尚未解锁目标情感动作、"
            + "你自己的某个模组已经修改了它，或者两个情感动作在转身、循环、音效或前奏方面不匹配。"),
        ("Added \"Support me on Ko-fi\" and \"Discord\" buttons to the main window.",
            "在主窗口中新增「在 Ko-fi 上支持我」和「Discord」按钮。"),
        ("Fixed \"Create a mod\" sometimes writing the wrong animation instead of the one you asked for.",
            "修复了「创建模组」有时写入错误动画、而非你指定动画的问题。"),
        ("Fixed changing world turning off the generated Penumbra mod and its options.",
            "修复了切换服务器会关闭生成的 Penumbra 模组及其选项的问题。"),
        ("Fixed one of the signatures the plugin needed for the cache-breaker feature.\n"
         + "Without it, a swap could play the emote you played before it.",
            "修复了缓存刷新功能所需的其中一个特征码。\n"
            + "缺少它时，替换可能会播放你在此之前播放过的情感动作。"),

        // ── 2.3.0.0 ───────────────────────────────────────────────────────────
        ("Animation refresh rework", "动画刷新机制重做"),
        ("Reworks how animations are refreshed, and stops other players from seeing you redraw.",
            "重做了动画刷新的方式，并避免其他玩家看到你重新加载角色。"),
        ("Added \"Add an override...\" to the right click menu of the main UI.\n"
         + "It opens the \"Emote overrides\" tab with the emote already picked.",
            "在主界面的右键菜单中新增「添加覆盖...」。\n"
            + "它会打开「情感动作覆写」标签页，并已预先选好该情感动作。"),
        ("\"Always cache-break\" no longer makes other players see you redraw for random emotes.",
            "「始终刷新缓存」不再让其他玩家在随机情感动作上看到你重新加载角色。"),
        ("Reworked how the plugin refreshes an animation, it is now way more efficient and optimized. Dropped from 10+ sigs to 2.",
            "重做了插件刷新动画的方式，现在效率与优化程度都大幅提升。特征码从 10 多个减少到 2 个。"),

        // ── 2.3.1.0 ───────────────────────────────────────────────────────────
        ("Idle pose fixes", "待机动作修复"),
        ("Fixes loop emotes playing your own idle mod instead of the emote you asked for.",
            "修复了循环情感动作播放你自己的待机模组、而不是你请求的情感动作的问题。"),
        ("Loop emotes no longer play your own idle mod instead of the emote you asked for.\n"
         + "Generated bypass mods priority was not correctly set, this is fixed now.",
            "循环情感动作不再播放你自己的待机模组，而是播放你请求的情感动作。\n"
            + "生成的无视限制模组优先级此前设置不正确，现已修复。"),
        ("Fixed a bug where stopping a bypass on an idle animation by moving would not correctly stop the animation.",
            "修复了通过移动来停止待机动画上的无视限制播放时、动画无法正确停止的问题。"),
        ("Fixed a bug with \"Only when nothing else fits\" for \"Loops on idle poses\" where it ignored blocked emotes.",
            "修复了「在待机动作上循环」的「仅当无其他选择」会忽略被屏蔽情感动作的问题。"),
        ("/belogs now exports everything the plugin did since it loaded.\n"
         + "It would only partially export logs when the dalamud log level was not properly set.",
            "/belogs 现在会导出插件自加载以来所做的全部内容。\n"
            + "当 Dalamud 日志级别设置不当时，它此前只会导出部分日志。"),
        ("The plugin reads its mod's priority back from Penumbra instead of taking the cached value.",
            "插件现在会从 Penumbra 读回其模组的优先级，而不是使用缓存的值。"),
    ];
}