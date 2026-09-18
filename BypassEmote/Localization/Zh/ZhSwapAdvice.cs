namespace BypassEmote.Localization;

/// <summary> 中文文案：交换建议与交换反馈（<c>EmoteSwap/Matching/SwapAdvice.cs</c>、<c>EmoteSwap/Orchestrator/SwapOrchestrator.Feedback.cs</c>）。 </summary>
internal static class ZhSwapAdvice
{
    internal static readonly (string En, string Zh)[] Entries =
    [
        // ── 无法完成替换时的聊天回执 ────────────────────────────────────────────
        ("Could not swap /{0}.", "无法替换 /{0}。"),
        ("Found /{0} but {1}", "找到 /{0}，但{1}"),
        ("Also found /{0} but {1}", "另外找到 /{0}，但{1}"),

        ("it is on your blocked targets list.", "它在你屏蔽的目标列表中。"),
        ("another of your mods targets it. Your configuration blocked it.",
            "你的另一个模组以它为目标，已被你的配置屏蔽。"),
        ("your mod \"{0}\" targets it. Your configuration blocked it.",
            "你的模组「{0}」以它为目标，已被你的配置屏蔽。"),
        ("the loop kinds do not match.", "两者的循环类型不匹配。"),
        ("turn matching is very strict.", "转身匹配设置为非常严格。"),
        ("it makes a sound, and your sound rule avoids every target that does.",
            "它会发出声音，而你的音效规则会避开所有会发声的目标。"),
        ("it makes a sound, and your sound rule only allows that for an emote that makes one too.",
            "它会发出声音，而你的音效规则只允许替换到同样会发声的情感动作。"),
        ("it makes a sound.", "它会发出声音。"),
        ("{0} matching is strict.", "{0}匹配设置为严格。"),
        // 运行时查表（L.T(blockedBy.ToLowerInvariant())），静态检查会报「未使用」，实际在用。
        ("loop", "循环"),
        ("turn", "转身"),
        ("sound", "音效"),

        // ── 覆写建议 ──────────────────────────────────────────────────────────
        ("{0} is the emote being bypassed.", "{0} 就是被无视限制播放的那个情感动作。"),
        ("{0} can never be a swap target: it is a pose, a facial expression, a per-job emote, or it draws your weapon. It is always skipped.",
            "{0} 永远不能作为替换目标：它是姿势、表情、职业专属动作，或者会拔出武器。它会被始终跳过。"),
        ("You have not unlocked {0}. It is skipped.", "你还没有解锁 {0}，将被跳过。"),
        ("{0} is on your blocked targets list. This override uses it anyway.",
            "{0} 在你屏蔽的目标列表中，但此覆写仍会使用它。"),
        ("Your mod \"{0}\" already changes {1}. What your \"Emotes your mods change\" setting is set to still applies here.",
            "你的模组「{0}」已经修改了 {1}。这里仍然会应用你在「被模组修改的情感动作」中的设置。"),

        // ── 姿态 ──────────────────────────────────────────────────────────────
        ("{0} and {1} share no posture. The swap will not happen.", "{0} 与 {1} 没有共同的姿态，替换不会发生。"),
        ("{0} has no {1} animation. The swap will not happen when you play {2} in that posture.",
            "{0} 没有{1}姿态的动画。当你以该姿态播放 {2} 时，替换不会发生。"),
        ("standing", "站立"),
        ("chair sitting", "坐在椅子上"),
        ("ground sitting", "坐在地上"),
        ("mounted", "骑乘"),
        ("matching", "共有姿态"),
        (" or ", "或"),

        // ── 行为差异 ──────────────────────────────────────────────────────────
        ("{0} will stop when you turn your character.", "当你转动角色时，{0} 会中断。"),
        ("{0} plays a separate animation when you use it on someone (adjust variant) but {1} has no adjust variant. Targeting someone will keep {2}'s animation.",
            "{0} 对他人使用时有一套单独的动画（adjust 变体），但 {1} 没有 adjust 变体。选中他人时会继续播放 {2} 的动画。"),
        ("{0} plays once while {1} loops. The animation will stop instead of looping.",
            "{0} 只播放一次，而 {1} 是循环的。动画会停止而不会循环。"),
        ("{0} loops while {1} plays once. Weird behavior might happen.",
            "{0} 是循环的，而 {1} 只播放一次。可能会出现异常表现。"),
        ("{0} does not behave like {1} when you target someone: {0} {2} while {1} {3}.",
            "{0} 与 {1} 在选中他人时的表现不同：{0}{2}，而 {1}{3}。"),
        ("does not turn at all", "完全不转身"),
        ("only follows with the eyes", "只转动视线"),
        ("turns the head", "转动头部"),
        ("turns the whole body", "转动整个身体"),
        ("turns in a way the plugin could not read", "以插件无法识别的方式转身"),

        // ── 音效与开场 ────────────────────────────────────────────────────────
        ("{0} emits a voice line sound. Everyone around you will hear it.", "{0} 会发出语音音效，周围的人都会听到。"),
        ("{0} emits a sound that {1} does not. Everyone around you will hear it.",
            "{0} 会发出 {1} 没有的音效，周围的人都会听到。"),
        ("{0} has no intro meanwhile {1} has one, meaning the intro will not play.",
            "{0} 没有开场动作，而 {1} 有，因此开场动作不会播放。"),
        ("{0} has an intro and {1} does not.", "{0} 有开场动作，而 {1} 没有。"),

        // ── 无法替换的原因（编排器 / 预览） ────────────────────────────────────
        ("Still loading emote data. Try again in a moment.", "情感动作数据仍在加载中，请稍后再试。"),
        ("Something went wrong. Emote not swapped.", "出了点问题，未执行替换。"),
        ("No Penumbra collection is assigned to your character. Emote not swapped.",
            "你的角色没有分配 Penumbra 集合，未执行替换。"),
        ("{0} Emote not swapped.", "{0} 未执行替换。"),
        ("Penumbra could not say which collection your character uses. Emote not swapped.",
            "Penumbra 无法确定你的角色使用哪个集合，未执行替换。"),
        ("Emote Swap is off.", "表情替换已关闭。"),
        ("Client in gpose.", "客户端处于 GPose 中。"),
        ("Local player not found.", "找不到本地玩家。"),
        ("This emote is not in the catalog.", "该情感动作不在目录中。"),
        ("/{0} needs {1}.", "/{0} 需要{1}。"),
        // 运行时查表（L.T(EmoteHelper.EnvironmentRequirementFor(...))）。
        ("water underfoot", "站在水中"),

        // ── Penumbra 不可用的原因（聊天 / 通知 / 窗口都会用到） ────────────────
        ("Penumbra is not running.", "Penumbra 未运行。"),
        ("Penumbra is not running. Emote Swap needs it installed.", "Penumbra 未运行。表情替换需要先安装它。"),
        ("Penumbra answers over interface version {0}, and Bypass Emote needs version {1} or newer. Update Penumbra.",
            "Penumbra 的接口版本为 {0}，而 Bypass Emote 需要 {1} 或更高版本。请更新 Penumbra。"),

        // ── 直接播放的拒绝原因 ────────────────────────────────────────────────
        ("/{0} cannot be played {1}.", "/{0} 无法在{1}播放。"),
        ("while carrying your {0}", "携带{0}时"),
        ("while standing", "站立时"),
        ("while swimming", "游泳时"),
        ("while diving", "潜水时"),
        ("while sitting on the ground", "坐在地上时"),
        ("while sitting in a chair", "坐在椅子上时"),
        ("while mounted", "骑乘时"),
        ("while holding an umbrella", "手持雨伞时"),
        ("while holding a torch", "手持火把时"),
        ("while wearing a fashion accessory", "佩戴时尚配饰时"),
        ("while fishing", "钓鱼时"),
        ("right now", "当前状态"),

        // ── 匹配器挡住候选时的标识（EmotePoolTab 会原样显示） ──────────────────
        // 这四条只在运行时查表（L.T(blockedBy.ToLowerInvariant())），源码里没有字面量调用点，
        // 所以静态覆盖率检查会把它们报成「未使用」——它们确实在用。
        ("rules", "规则"),
        ("modded", "模组"),

        // ── 情感动作池「状态」下拉的枚举名 ────────────────────────────────────
        ("Standing", "站立"),
        ("SittingInChair", "坐在椅子上"),
        ("SittingOnGround", "坐在地上"),
        ("Mounted", "骑乘"),
        ("Swimming", "游泳"),
        ("Diving", "潜水"),
        ("Fishing", "钓鱼"),
        ("HoldingUmbrella", "手持雨伞"),
        ("HoldingTorch", "手持火把"),
        ("WearingFashionAccessory", "佩戴时尚配饰"),

        // ── 待机动作兜底（SwapOrchestrator.IdlePose） ─────────────────────────
        ("idle pose", "待机动作"),
        ("Your character is still in another emote. Move, or change pose, then try again.",
            "你的角色仍在播放其他情感动作。请移动或更换姿势后重试。"),
        ("Your character is mounted, so there is no idle pose to borrow.",
            "你的角色正在骑乘，没有可借用的待机动作。"),
        ("This pose cannot be changed.", "该姿势无法更改。"),
        ("Your pose animation could not be found.", "找不到你的姿势动画。"),
        ("That emote has no animation to lend.", "该情感动作没有可借出的动画。"),
        ("That emote's animation could not be found.", "找不到该情感动作的动画。"),
        ("Your pose animation could not be rebuilt.", "你的姿势动画无法重建。"),
        ("Penumbra could not say which collection your character uses.", "Penumbra 无法确定你的角色使用哪个集合。"),
        ("The swap mod could not be turned on.", "无法开启替换模组。"),
        ("Your character could not be refreshed.", "无法重新绘制你的角色。"),
        ("Something went wrong.", "出了点问题。"),
        ("Could not use your idle pose for this emote. {0}", "无法为此情感动作使用你的待机动作。{0}"),
        ("Your idle 0 pose has no intro, so this emote's intro will not play. Try changing pose.",
            "你的待机 0 号姿势没有开场动作，因此该情感动作的开场不会播放。请尝试更换姿势。"),

        // ── 调试日志导出（Helpers/DebugLogExporter） ──────────────────────────
        ("A debug log export is already running.", "调试日志导出已在进行中。"),
        ("Exporting logs...", "正在导出日志……"),
        ("Debug logs could not be exported. The Dalamud log has the reason.",
            "无法导出调试日志，原因见 Dalamud 日志。"),
        ("Debug logs exported to ", "调试日志已导出到 "),
        (". Send this file to the developer. Feel free to check the content of the zip and if you need to anonymize any information, please do so before sending it. ",
            "。请把这个文件发送给开发者。你可以先查看压缩包内容；如需匿名化处理，请在发送前完成。 "),
        ("Personal information appears in it, DO NOT send this in a public channel.",
            "其中包含个人信息，切勿在公开频道发送。"),
        (" Ask the developer where to send this file to be extra safe.", " 为了更安全，请先向开发者确认发送途径。"),
        ("[Open folder]", "[打开文件夹]"),

        // ── 覆写找不到可用目标 / 落点被模组修改 / 开场被丢弃 ────────────────────
        ("Emote Swap was enabled because no choice was made.", "由于没有做出选择，已启用表情替换。"),
        ("Could not swap /{0}: none of the target override emotes can be played right now.",
            "无法替换 /{0}：目标覆写的情感动作目前都无法播放。"),
        ("{0} was skipped because {1}.", "{0} 被跳过，因为{1}。"),
        ("{0} more target(s) were skipped for the same reason.", "还有 {0} 个目标因相同原因被跳过。"),
        ("Emote #{0}", "情感动作 #{0}"),
        ("you have not unlocked it", "你还没有解锁它"),
        ("it can never be a swap target", "它永远不能作为替换目标"),
        ("it cannot be played in your current state", "它无法在你当前的状态下播放"),
        ("one of your mods changes it, and your settings block those", "你的某个模组修改了它，而你的设置屏蔽了这类目标"),
        ("there is no animation data for it", "它没有动画数据"),
        ("it is available", "它可以替换"),
        ("This emote landed on /{0}, which your mod \"{1}\" changes. Players around you may briefly see that mod's animation before yours reaches them. If you don't want this to happen, head over to the configuration window and block emotes that are changed by other mods.",
            "该情感动作替换到了 /{0}，而你的模组「{1}」修改了它。你周围的玩家可能会先短暂看到该模组的动画，之后才会收到你的。如果不希望这样，请到配置窗口中屏蔽被其他模组修改的情感动作。"),
        ("This emote landed on a {0} target with no intro. You will not see the intro play.",
            "该情感动作替换到了没有开场动作的{0}目标上，你不会看到开场播放。"),
        ("loop only", "仅循环"),
        ("one shot", "一次性"),

        // ── 创建独立模组的结果（EmoteSwap/Mod/PermanentModBuilder，显示在创建窗口状态行） ──
        ("Something went wrong. Nothing was created; the log has the details.",
            "出了点问题，没有创建任何内容；详情见日志。"),
        ("Pick at least one race for the mod to cover.", "请至少选择一个该模组要覆盖的种族。"),
        ("Give the mod a name first.", "请先给模组起个名字。"),
        ("Penumbra's mod folder could not be read.", "无法读取 Penumbra 的模组目录。"),
        ("The swap engine is not running.", "替换引擎未运行。"),
        ("Penumbra already holds a mod folder called '{0}'. Pick another name.",
            "Penumbra 中已存在名为「{0}」的模组目录，请换一个名字。"),
        ("/{0} cannot be played over /{1}: they share no posture to move the animation onto.",
            "/{0} 无法覆盖 /{1}：两者没有可用于迁移动画的共同姿态。"),
        ("The mod's files could not be written. The log has the details.", "无法写入模组文件，详情见日志。"),
        ("'{0}' was written to '{1}' but Penumbra would not take it. Rediscovering mods in Penumbra should pick it up.",
            "「{0}」已写入「{1}」，但 Penumbra 没有接受它。在 Penumbra 中重新扫描模组即可加载。"),
        ("'{0}' was created. Enable it in Penumbra when you want it.", "「{0}」已创建。需要时在 Penumbra 中启用它。"),
        ("'{0}' was created, but no collection is assigned to your character, so it is off.",
            "「{0}」已创建，但你的角色没有分配集合，因此它是关闭的。"),
        ("'{0}' was created, but Penumbra would not switch it on in {1}.",
            "「{0}」已创建，但 Penumbra 无法在 {1} 中启用它。"),
        ("'{0}' was created and switched on in {1}.", "「{0}」已创建，并已在 {1} 中启用。"),
    ];
}