using BypassEmote.EmoteSwap;
using BypassEmote.Enums;
using BypassEmote.Helpers;
using BypassEmote.Safety;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using NoireLib;
using NoireLib.Changelog;
using NoireLib.Helpers;
using NoireLib.Helpers.ObjectExtensions;
using NoireLib.UI;
using NoireLib.UpdateTracker;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace BypassEmote.UI;

public class ConfigWindow : Window, IDisposable
{
    private static string[] SwapLifetimeOptions =>
        [L.T("When the emote ends"), L.T("When you play the target emote"), L.T("Never")];

    private static string[] SwapBehaviorOptions => [L.T("Multiple swaps"), L.T("One swap at a time")];

    private static string[] LoopMatchingOptions => [L.T("Strict"), L.T("Lenient")];
    private static string[] SoundMatchingOptions => [L.T("Strict"), L.T("Lenient"), L.T("Off")];
    private static string[] TurnMatchingOptions => [L.T("Very strict"), L.T("Strict"), L.T("Lenient")];
    private static readonly TurnMatchRule[] TurnMatchingOrder = [TurnMatchRule.VeryStrict, TurnMatchRule.Strict, TurnMatchRule.Lenient];

    private static string[] ModdedTargetsOptions => [L.T("Allowed"), L.T("Last resort"), L.T("Blocked")];
    private static string[] IdlePoseLoopsOptions => [L.T("Never"), L.T("Only when nothing else fits"), L.T("Allow")];

    private static string[] CachedDispatchOptions => [L.T("Off"), L.T("Only when necessary"), L.T("On")];

    private static string[] DispatchFidelityOptions => [L.T("Same rank only"), L.T("One rank below"), L.T("Anything allowed")];

    private static string EmoteSwapLabel => L.T("Emote Swap");
    private static string DirectPlayLabel => L.T("Direct Play");
    private static string[] ModeOptions => [EmoteSwapLabel, DirectPlayLabel];

    private static string ModeName => L.T("Mode");
    private static string LifetimeName => L.T("Turn the swap off");
    private static string BehaviorName => L.T("Swaps at the same time");
    private static string KeptSwapsName => L.T("Kept swaps per emote");
    private static string AnonymizeModName => L.T("Hide your name on the mod");
    private static string AlwaysCacheBreakName => L.T("Always cache-break (experimental)");
    private static string LoopMatchingName => L.T("Loop matching");
    private static string TurnMatchingName => L.T("Turn matching");
    private static string SoundMatchingName => L.T("Sound matching");
    private static string CachedDispatchName => L.T("Spread swaps over several emotes");
    private static string MaxTargetsName => L.T("Emotes used per behaviour");
    private static string DispatchFidelityName => L.T("How close a spread emote must fit");
    private static string ErrorThrottleName => L.T("Repeat an error at most every");
    private static string WarningThrottleName => L.T("Repeat a warning at most every");

    private static string UnsafeToggleName => L.T("Unsafe toggle");

    private static string ModdedTargetsName => L.T("Emotes your mods change");
    private static string IdlePoseLoopsName => L.T("Loops on idle poses");
    private static string SwapMessagesName => L.T("Show swap messages");
    private static string ErrorMessagesName => L.T("Show error messages");
    private static string WarningMessagesName => L.T("Show warning messages");
    private static string FaceTargetName => L.T("Face your target automatically");

    private static string[] SwapNames =>
        [ModeName, LifetimeName, BehaviorName, KeptSwapsName, AnonymizeModName, LoopMatchingName, TurnMatchingName, SoundMatchingName,
         CachedDispatchName, MaxTargetsName, DispatchFidelityName, ErrorThrottleName, WarningThrottleName,
         ModdedTargetsName, IdlePoseLoopsName, AlwaysCacheBreakName,
         SwapMessagesName, ErrorMessagesName, WarningMessagesName, FaceTargetName, UnsafeToggleName];

    private static string PluginEnabledName => L.T("Enable the plugin");
    private static string HotbarBypassName => L.T("Bypass emotes from locked hotbar slots");
    private static string LockedEmotesInWindowName => L.T("Show locked emotes in the game's Emote window");
    private static string LockedEmotesName => L.T("Do not grey out locked emotes");
    private static string StopOnMoveName => L.T("Stop companion emotes when they move");
    private static string UpdateNotificationName => L.T("Show update notifications");
    private static string ChangelogName => L.T("Show the changelog after an update");
    private static string GposeWindowsName => L.T("Show windows in GPose");
    private static string HiddenUiWindowsName => L.T("Show windows while the game UI is hidden");

    private static string[] GeneralNames =>
        [PluginEnabledName, HotbarBypassName, LockedEmotesInWindowName, LockedEmotesName, StopOnMoveName,
            UpdateNotificationName, ChangelogName,
            GposeWindowsName, HiddenUiWindowsName];

    private static readonly DurationStyle WarningThrottleStyle = new()
    {
        Hint = "5m",
        BareUnit = DurationUnit.Seconds,
        Min = TimeSpan.Zero,
        Max = TimeSpan.FromHours(1),
        Default = TimeSpan.FromMinutes(5),
        Width = 0f,
        ShowPreview = false,
        Focus = new FocusStyle { Shape = FocusShape.None },
    };

    private static readonly DurationStyle ErrorThrottleStyle = new()
    {
        Hint = "0s",
        BareUnit = DurationUnit.Seconds,
        Min = TimeSpan.Zero,
        Max = TimeSpan.FromHours(1),
        Default = TimeSpan.Zero,
        Width = 0f,
        ShowPreview = false,
        Focus = new FocusStyle { Shape = FocusShape.None },
    };

    private static readonly NumberStyle MaxTargetsStyle = new()
    {
        Step = 1f,
        FastStep = 5f,
        Min = 1f,
        Max = 50f,
        Default = 3f,
        Width = 0f,
        Focus = new FocusStyle { Shape = FocusShape.None },
    };

    private static readonly NumberStyle KeptSwapsStyle = new()
    {
        Step = 1f,
        FastStep = 5f,
        Min = 0f,
        Max = 100f,
        Default = 5f,
        Width = 0f,
        Focus = new FocusStyle { Shape = FocusShape.None },
    };

    private const string GeneralTabId = "general";
    private const string ModeTabId = "mode";
    private const string OverridesTabId = "overrides";

    private const float SettingsContentWidth = 485f;

    private const float WarningCountdownSeconds = 5f;

    private static readonly TimeSpan UnsafeAttentionDuration = TimeSpan.FromSeconds(5);

    private static string SyncServicesLine => L.T("Not all sync services support Direct Play.");

    private static string SafeModeLimitLine => L.T(
        "In safe mode you can only bypass emotes from the base pose (pose 0) of your current stance.");

    private static string SafeModeIsNotAPromiseLine => L.T(
        "Safe mode is not 100% guaranteed to be safe either. It prevents Direct Play from being used in states where it was proved to "
        + "go wrong, but I can not prove the absence of issues. Use Emote Swap if you want to be 100% safe, it will behave almost the same way.");

    private static string UnsafeHeadline => L.T(
        "This is unsafe. Forcing an emote outside your base pose is, in theory, detectable by the server.");

    private static string UnsafeReassurance => L.T(
        "In practice it is a non-issue. This has been a thing in other tools and plugins (and still is in some of them), "
        + "which people have used for years without trouble. Go back to safe mode, or even better, to emote swap, if you are uncomfy with this.");

    private static string UnsafeToggleHelp => L.T(
        "Lets Direct Play bypass an emote whatever pose you are in."
        + "\n\nLeave it off unless you know what you are doing: off, the plugin only plays emotes from states where "
        + "nothing can be noticed.");

    private static string SafeDirectPlayTooltip => L.T(
        "Not all sync services support it. In safe mode you can only bypass emotes from the base pose (pose 0) of your current stance.");

    private static string UnsafeDirectPlayTooltip => L.T(
        "Not recommended. Not all sync services support it. Emote Swap is safer and works over any sync service.");

    private static string ModeHelp => L.T(
        "\"Emote Swap plays\" your emote over one your character owns, through a Penumbra mod. Other players see it "
        + "over any sync service."
        + "\n\"Direct Play\" sends the emote to the game itself, which not every sync service supports.");

    public ConfigWindow() : base("Bypass Emote##BypassEmoteConfig",
        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(495, 400),
            MaximumSize = new Vector2(495, float.MaxValue)
        };

        TitleBarButtons.Add(new()
        {
            Click = (m) => { if (m == ImGuiMouseButton.Left) Service.OpenKofi(); },
            Icon = FontAwesomeIcon.Heart,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip(L.T("Support me")),
        });
    }

    private static readonly NoireTabBar Tabs = new("BypassEmoteConfig")
    {
        Tabs =
        {
            new UiTab(GeneralTabId, L.T("General settings"), () => DrawTabBody("##BypassEmoteGeneralBody", DrawGeneralSettings)),
            new UiTab(ModeTabId, L.T("Bypass Mode"), () => DrawTabBody("##BypassEmoteModeBody", DrawBypassMode)),
            new UiTab(OverridesTabId, L.T("Emote overrides"), () => DrawWideTabBody("##BypassEmoteOverridesBody", OverridesTab.Draw)),
        },
    };

    private static DateTime unsafeAttentionUntil = DateTime.MinValue;

    private static bool confirmingUnsafe;

    /// <summary>
    /// The tab bar is built once, so its labels would otherwise freeze at the language that was active on first use.
    /// UiTab.Label is writable, so they are refreshed every frame instead of rebuilding the bar (which would reset
    /// the selected tab).
    /// </summary>
    private static void RefreshTabLabels()
    {
        foreach (var tab in Tabs.Tabs)
        {
            tab.Label = tab.Id switch
            {
                GeneralTabId => L.T("General settings"),
                ModeTabId => L.T("Bypass Mode"),
                OverridesTabId => L.T("Emote overrides"),
                _ => tab.Label,
            };
        }
    }

    public override void Draw()
    {
        RefreshTabLabels();

        DrawPatchApproval();

        Tabs.Draw();
    }

    public static void SwitchToBypassMode() => Tabs.SwitchTab(ModeTabId);

    public static void SwitchToOverrides(uint sourceRowId)
    {
        OverridesTab.ShowFor(sourceRowId);
        Tabs.SwitchTab(OverridesTabId);
    }

    public static void ShowUnsafeToggleAttention() => unsafeAttentionUntil = DateTime.UtcNow + UnsafeAttentionDuration;

    private static readonly Vector4 PatchWarningColor = ColorHelper.HexToVector4("#E81313");
    private static readonly Vector4 PatchNoticeColor = ColorHelper.HexToVector4("#FF8C1A");

    private static bool checkingApproval;

    private static string CheckNowLabel => L.T("Check now");

    private static void DrawPatchApproval()
    {
        var gate = Service.PatchApproval;

        if (gate == null)
            return;

#if DEBUG
        if (gate.ForcedApproval)
        {
            DrawForcedApproval(gate);
            return;
        }
#endif

        if (gate.Untested)
        {
            DrawUntestedClient(gate);
            return;
        }

        if (!gate.Governs || gate.Approved)
            return;

        ImGui.TextColoredWrapped(PatchWarningColor, L.T("Bypass Emote has not been approved for this game build."));
        ImGui.TextWrapped(gate.Reason);

        ImGui.TextWrapped(L.T("Emote swaps may behave oddly until the build is approved.\nThe plugin will automatically fetch updates every 10 minutes to check if it was approved."));

        if (gate.Notice is { Length: > 0 } notice)
            ImGui.TextColoredWrapped(PatchNoticeColor, notice);

        var checkedAt = gate.LastCheckedUtc is { } utc
            ? utc.ToLocalTime().ToString("HH:mm:ss")
            : L.T("not yet");

        ImGui.TextDisabled(L.T("Checked at {0}.", checkedAt));

        DrawCheckNowButton(gate);

#if DEBUG
        ImGui.SameLine();

        if (ImGui.Button(L.T("Enable Force Approval") + "##BypassEmoteForceApproval"))
            gate.ForceApproval(true);
#endif

        ImGui.Separator();
    }

#if DEBUG
    private static readonly Vector4 ForcedApprovalColor = ColorHelper.HexToVector4("#3FBF7F");

    private static void DrawForcedApproval(PatchApprovalGate gate)
    {
        ImGui.TextColoredWrapped(ForcedApprovalColor, L.T("Force approval is on."));

        if (ImGui.Button(L.T("Disable Force Approval") + "##BypassEmoteForceApproval"))
            gate.ForceApproval(false);

        ImGui.Separator();
    }
#endif

    private static void DrawUntestedClient(PatchApprovalGate gate)
    {
        // The reason itself ("this client has not and can not be tested") is a diagnostic and still reaches the
        // debug window through gate.Reason. What a CN user needs here is where this build came from and where to
        // report a problem, so this shows the branch notice instead: the warning on its own line, then the branch
        // credit and the link in green so it reads as an invitation rather than as part of the warning.
        ImGui.TextColoredWrapped(PatchWarningColor, L.T("The Bypass Emote you are using is not the original plugin."));

        ImGui.TextColoredWrapped(NoireTheme.Current.Resolve(ThemeColor.Success),
            L.T("This Chinese localization and CN client adaptation branch is maintained by Sachimi. "
                + "If you have any problem, please report the bug at https://github.com/Luckyumimi/BypassEmoteCN"));

        if (ImGui.Button(L.T("Open the project page") + "##BypassEmoteIssueTracker"))
            Service.OpenIssueTracker();

        if (gate.Notice is { Length: > 0 } notice)
            ImGui.TextColoredWrapped(PatchNoticeColor, notice);

        ImGui.Separator();
    }

    private static void DrawCheckNowButton(PatchApprovalGate gate)
    {
        var cooldown = gate.ManualCooldownSeconds;

        var label = cooldown > 0
            ? $"{CheckNowLabel} ({cooldown})"
            : CheckNowLabel;

        var width = ImGui.CalcTextSize($"{CheckNowLabel} ({PatchApprovalGate.ManualCheckCooldown.TotalSeconds:0})").X
            + (ImGui.GetStyle().FramePadding.X * 2f);

        using (ImRaii.Disabled(checkingApproval || cooldown > 0))
        {
            if (ImGui.Button($"{label}##BypassEmotePatchApproval", new Vector2(width, 0f)))
                _ = CheckApprovalAsync(gate);
        }
    }

    private static async Task CheckApprovalAsync(PatchApprovalGate gate)
    {
        checkingApproval = true;

        try
        {
            await gate.CheckNowAsync();
        }
        finally
        {
            checkingApproval = false;
        }
    }

    private static void DrawTabBody(string id, Action body)
    {
        var avail = ImGui.GetContentRegionAvail();
        var width = MathF.Min(avail.X, NoireUI.Scaled(SettingsContentWidth));

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + MathF.Max(0f, (avail.X - width) * 0.5f));

        using var child = ImRaii.Child(id, new Vector2(width, 0f), false);

        if (child)
            body();
    }

    private static void DrawWideTabBody(string id, Action body)
    {
        using var child = ImRaii.Child(id, Vector2.Zero, false);

        if (child)
            body();
    }

    private static void DrawGeneralSettings()
    {
        var names = SettingsLayout.NameColumn(GeneralNames);
        var controls = SettingsLayout.CheckboxColumn();

        DrawLanguageRow();

        SettingsLayout.Heading(L.T("Plugin"));

        using (var rows = SettingsLayout.Rows("##BypassEmotePluginRows", names, controls))
        {
            if (rows)
            {
                var pluginEnabled = Configuration.PluginEnabled;
                if (CheckRow(PluginEnabledName, ref pluginEnabled,
                    L.T("Lets you bypass emotes with the vanilla emote commands (/beesknees, /tea, and so on) and with hotbar slots."
                    + "\nThe main window and the /be command work regardless of this setting.")))
                {
                    Configuration.PluginEnabled = pluginEnabled;
                }

                var bypassOnHotbarSlotTriggered = Configuration.BypassOnHotbarSlotTriggered;
                if (CheckRow(HotbarBypassName, ref bypassOnHotbarSlotTriggered,
                    L.T("Bypasses a locked emote when you press its hotbar slot."
                    + "\nRight-click an emote in the main window to assign it to a slot.")))
                {
                    Configuration.BypassOnHotbarSlotTriggered = bypassOnHotbarSlotTriggered;
                }

                var showLockedEmotesInGameWindow = Configuration.ShowLockedEmotesInGameWindow;
                if (CheckRow(LockedEmotesInWindowName, ref showLockedEmotesInGameWindow,
                    L.T("Lists the emotes you have not unlocked in the game's own emote window."
                    + "\nClose and reopen the emote window for changes to appear.")))
                {
                    Configuration.ShowLockedEmotesInGameWindow = showLockedEmotesInGameWindow;
                }

                var showLockedEmotesAsUsable = Configuration.ShowLockedEmotesAsUsable;
                if (CheckRow(LockedEmotesName, ref showLockedEmotesAsUsable,
                    L.T("Keeps the emotes you have not unlocked lit in the game's emote window and on your hotbars."
                    + "\nPurely cosmetic.")))
                {
                    Configuration.ShowLockedEmotesAsUsable = showLockedEmotesAsUsable;
                }

                var stopCompanionEmoteOnCompanionMove = Configuration.StopOwnedObjectEmoteOnMove;
                if (CheckRow(StopOnMoveName, ref stopCompanionEmoteOnCompanionMove,
                    L.T("Stops your minion, pet or chocobo's looped emote when they move.")))
                {
                    Configuration.StopOwnedObjectEmoteOnMove = stopCompanionEmoteOnCompanionMove;
                    IpcHelper.NotifyConfigChanged();
                }
            }
        }

        SettingsLayout.Heading(L.T("Windows"));

        using (var rows = SettingsLayout.Rows("##BypassEmoteWindowRows", names, controls))
        {
            if (rows)
            {
                var showWindowsInGpose = Configuration.ShowWindowsInGpose;
                if (CheckRow(GposeWindowsName, ref showWindowsInGpose,
                    L.T("Keeps this plugin's windows visible while you are in GPose.")))
                {
                    Configuration.ShowWindowsInGpose = showWindowsInGpose;
                    Plugin.ApplyUiHideFlags();
                }

                var showWindowsWhenUiHidden = Configuration.ShowWindowsWhenUiHidden;
                if (CheckRow(HiddenUiWindowsName, ref showWindowsWhenUiHidden,
                    L.T("Keeps this plugin's windows visible when you hide the game UI.")))
                {
                    Configuration.ShowWindowsWhenUiHidden = showWindowsWhenUiHidden;
                    Plugin.ApplyUiHideFlags();
                }
            }
        }

        SettingsLayout.Heading(L.T("Updates"));

        using (var rows = SettingsLayout.Rows("##BypassEmoteUpdateRows", names, controls))
        {
            if (rows)
            {
                var showUpdateNotification = Configuration.ShowUpdateNotification;
                if (CheckRow(UpdateNotificationName, ref showUpdateNotification,
                    L.T("Tells you in chat and on screen when a new version of the plugin has been installed.")))
                {
                    Configuration.ShowUpdateNotification = showUpdateNotification;
                    var updateTracker = NoireLibMain.GetModule<NoireUpdateTracker>();
                    updateTracker?.SetShouldShowNotificationOnUpdate(Configuration.ShowUpdateNotification);
                    updateTracker?.SetShouldPrintMessageInChatOnUpdate(Configuration.ShowUpdateNotification);
                }

                var showChangelogOnUpdate = Configuration.ShowChangelogOnUpdate;
                if (CheckRow(ChangelogName, ref showChangelogOnUpdate,
                    L.T("Opens the changelog window after an update.")))
                {
                    Configuration.ShowChangelogOnUpdate = showChangelogOnUpdate;
                    var changelogManager = NoireLibMain.GetModule<NoireChangelogManager>();
                    changelogManager?.SetAutomaticallyShowChangelog(Configuration.ShowChangelogOnUpdate);
                }
            }
        }
    }

    private static string LanguageName => L.T("Language");

    private static string[] LanguageOptions => [L.T("Follow the game"), "简体中文", "English"];

    private static void DrawLanguageRow()
    {
        SettingsLayout.Heading(LanguageName);

        using var rows = SettingsLayout.Rows("##BypassEmoteLanguageRows",
            SettingsLayout.NameColumn(LanguageName),
            SettingsLayout.ControlColumn(LanguageOptions));

        if (!rows)
            return;

        SettingsLayout.Name(LanguageName);

        var language = (int)Configuration.Language;

        if (ImGui.Combo("##BypassEmoteLanguage", ref language, LanguageOptions, LanguageOptions.Length))
        {
            Configuration.Language = (PluginLanguage)language;
            L.Apply(Configuration.Language);
        }

        SettingsLayout.Help(L.T("Language used by this plugin's windows and chat messages."));
    }

    private static void DrawBypassMode()
    {
        var names = SettingsLayout.NameColumn(SwapNames);
        var controls = SettingsLayout.ControlColumn(
            ModeOptions, SwapLifetimeOptions, SwapBehaviorOptions, LoopMatchingOptions, TurnMatchingOptions,
            SoundMatchingOptions, DispatchFidelityOptions, ModdedTargetsOptions, IdlePoseLoopsOptions);

        using (var rows = SettingsLayout.Rows("##BypassEmoteModeRow", names, controls))
        {
            if (rows)
            {
                SettingsLayout.Name(ModeName);
                DrawModeCombo();
                SettingsLayout.Help(ModeHelp);
            }
        }

        if (Configuration.SelfBypassMode == SelfBypassMode.EmoteSwap)
        {
            DrawEmoteSwapSettings(names, controls);
            return;
        }

        DrawDirectPlaySettings(names, controls);
    }

    private static void DrawModeCombo()
    {
        var current = Configuration.SelfBypassMode;
        var directPlayActive = current == SelfBypassMode.DirectPlay;
        var previewLabel = directPlayActive ? DirectPlayLabel : EmoteSwapLabel;

        if (directPlayActive)
            ImGui.PushStyleColor(ImGuiCol.Text, NoireTheme.Current.Resolve(ThemeColor.Danger));

        var comboOpen = ImGui.BeginCombo("##BypassEmoteMode", previewLabel);

        if (directPlayActive)
            ImGui.PopStyleColor();

        if (!comboOpen)
            return;

        var emoteSwapSelected = current == SelfBypassMode.EmoteSwap;
        if (ImGui.Selectable(EmoteSwapLabel, emoteSwapSelected) && !emoteSwapSelected)
            ModeSwitcher.Apply(SelfBypassMode.EmoteSwap);

        if (emoteSwapSelected)
            ImGui.SetItemDefaultFocus();

        ImGui.PushStyleColor(ImGuiCol.Text, NoireTheme.Current.Resolve(ThemeColor.Danger));
        var directPlayClicked = ImGui.Selectable(DirectPlayLabel, directPlayActive);
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(Configuration.DirectPlayUnsafe ? UnsafeDirectPlayTooltip : SafeDirectPlayTooltip);
            ImGui.EndTooltip();
        }

        if (directPlayClicked && !directPlayActive)
            _ = ConfirmSwitchToDirectPlayAsync();

        if (directPlayActive)
            ImGui.SetItemDefaultFocus();

        ImGui.EndCombo();
    }

    private static void DrawDirectPlaySettings(float names, float controls)
    {
        SettingsLayout.Heading(L.T("Safety"));

        using (var rows = SettingsLayout.Rows("##BypassEmoteDirectPlaySafetyRows", names, controls))
        {
            if (rows)
                DrawUnsafeToggleRow();
        }

        DrawSafetyNotice();

        SettingsLayout.Heading(L.T("Direct play"));

        using (var rows = SettingsLayout.Rows("##BypassEmoteDirectPlayRows", names, controls))
        {
            if (!rows)
                return;

            var autoFaceTarget = Configuration.AutoFaceTargetDirectPlay;
            if (CheckRow(FaceTargetName, ref autoFaceTarget, L.T("Turns your character toward your target when you bypass an emote.")))
                Configuration.AutoFaceTargetDirectPlay = autoFaceTarget;
        }
    }

    private static void DrawUnsafeToggleRow()
    {
        var unsafeEnabled = Configuration.DirectPlayUnsafe;

        if (SettingsLayout.Check(UnsafeToggleName, ref unsafeEnabled))
        {
            if (unsafeEnabled)
                _ = ConfirmEnableUnsafeAsync();
            else
                Configuration.DirectPlayUnsafe = false;
        }

        NoireAttention.Glow(DateTime.UtcNow < unsafeAttentionUntil);

        SettingsLayout.Help(UnsafeToggleHelp);
    }

    private static void DrawSafetyNotice()
    {
        ImGui.Spacing();

        if (!Configuration.DirectPlayUnsafe)
        {
            var warningColor = NoireTheme.Current.Resolve(ThemeColor.Warning);

            ImGui.TextColoredWrapped(
                ColorHelper.ScaleAlpha(warningColor, NoireAttention.Pulse()), DirectPlayGate.SafeModeMessage);

            ImGui.Spacing();

            ImGui.TextColoredWrapped(
                NoireTheme.Current.Resolve(ThemeColor.TextMuted), SafeModeIsNotAPromiseLine);

            return;
        }

        var danger = ColorHelper.ScaleAlpha(NoireTheme.Current.Resolve(ThemeColor.Danger), NoireAttention.Pulse());
        var icon = FontAwesomeIcon.ExclamationTriangle.ToIconString();
        var top = ImGui.GetCursorPosY();

        Vector2 iconSize;

        using (ImRaii.PushFont(UiBuilder.IconFont))
            iconSize = ImGui.CalcTextSize(icon);

        ImGui.SetCursorPosY(top + MathF.Max(0f, NoireText.CenterOffset(TextSize.Heading) - (iconSize.Y * 0.5f)));

        using (ImRaii.PushFont(UiBuilder.IconFont))
            ImGui.TextColored(danger, icon);

        ImGui.SameLine();
        ImGui.SetCursorPosY(top);

        using (ImRaii.PushColor(ImGuiCol.Text, danger))
            NoireText.Wrapped(ImGui.GetContentRegionAvail().X, UnsafeHeadline, TextSize.Heading);

        ImGui.Spacing();
        ImGui.TextWrapped(UnsafeReassurance);
    }

    private static void DrawEmoteSwapSettings(float names, float controls)
    {
        SettingsLayout.Heading(L.T("Matching"));

        using (var rows = SettingsLayout.Rows("##BypassEmoteMatchingRows", names, controls))
        {
            if (rows)
            {
                var loopMatching = (int)Configuration.LoopMatching;
                if (ComboRow(LoopMatchingName, "##BypassEmoteLoopMatching", ref loopMatching, LoopMatchingOptions,
                    L.T("\"Strict\" only puts a looping emote on another looping one."
                    + "\n\"Lenient\" lets a looping emote play once on a one time emote when no better match exists."
                    + "\n\nRecommended: \"Strict\", or \"Lenient\" if you really don't have many emotes.")))
                {
                    Configuration.LoopMatching = (LoopMatchRule)loopMatching;
                }

                var turnMatching = Array.IndexOf(TurnMatchingOrder, Configuration.TurnMatching);
                if (ComboRow(TurnMatchingName, "##BypassEmoteTurnMatching", ref turnMatching, TurnMatchingOptions,
                    L.T("Emotes have different turn behaviors when you target someone. Some emotes will make your torso turn (i.e: /hum), some only your head (i.e: /stepdance),"
                    + "some will only make your eyes follow your target (i.e: /beesknees) while others will not move at all (i.e:/guard)."
                    + "\n\n\"Very strict\" only picks emotes that behaves the same way."
                    + "\n\"Strict\" allows eye following differences but keeps emotes head and body turn behaviors."
                    + "\n\"Lenient\" allows any turn behavior."
                    + "\n\nThe plugin will still always try to find the best match first, regardless of the selected rule."
                    + "\n\nRecommended: \"Lenient\".")))
                {
                    Configuration.TurnMatching = TurnMatchingOrder[turnMatching];
                }

                var soundMatching = (int)Configuration.SoundMatching;
                if (ComboRow(SoundMatchingName, "##BypassEmoteSoundMatching", ref soundMatching, SoundMatchingOptions,
                    L.T("\"Strict\" never puts an emote on one that makes sound."
                    + "\n\"Lenient\" allows matching emotes that make sounds together."
                    + "\n\"Off\" will let emotes play regardless of sound."
                    + "\n\nThis is to prevent vanilla people from seeing you play fume which could annoy other vanilla players, for example."
                    + "\n\nRecommended: \"Lenient\".")))
                {
                    Configuration.SoundMatching = (SoundMatchRule)soundMatching;
                }

                // Asked every frame: a hook can stop resolving mid-session.
                var cachedDispatch = (int)Configuration.CachedDispatch;
                if (ComboRow(CachedDispatchName, "##BypassEmoteCachedDispatch", ref cachedDispatch, CachedDispatchOptions,
                    L.T("Gives each bypassed emote a target emote of its own. This is useful when you want to bypass multiple emotes quickly."
                    + "\n\n\"Off\" would make it so other people "
                    + "on your sync service would see you redraw constantly."
                    + "\n\"Only when necessary\" spreads nothing while the game plays fresh content on its own, and "
                    + "steps in for the emotes that would show a stale frame once a game patch breaks that."
                    + "\n\"On\" always spreads emotes."
                    + "\n\nRecommended: \"On\" if you want other people on your sync service to always see you properly without "
                    + "redrawing all the time, otherwise highly recommended to leave it on \"Only when necessary\" and not \"Off\"."),
                    CachedDispatchAlarm()))
                {
                    Configuration.CachedDispatch = (CachedDispatchMode)cachedDispatch;
                }

                SettingsLayout.Name(MaxTargetsName);

                var maxTargets = Configuration.MaxTargetsPerRank;
                if (NoireInputs.Number("###BypassEmoteMaxTargets", ref maxTargets, MaxTargetsStyle))
                    Configuration.MaxTargetsPerRank = maxTargets;

                SettingsLayout.Help(L.T("How many different target emotes one kind of emote may swap to."));

                var dispatchFidelity = (int)Configuration.DispatchFidelity;
                if (ComboRow(DispatchFidelityName, "##BypassEmoteDispatchFidelity", ref dispatchFidelity,
                    DispatchFidelityOptions,
                    L.T("Takes effect when \"Spread swaps over several emote\" is enabled. This determines which emotes become available for a source emote. "
                    + "Basically, if you want to spread swaps over 5 emotes, and you try to bypass an emote but you only have 2 same-rank targets available, "
                    + "this is how it will determine what to do in this scenario. A rank is basically a category of emotes with similar characteristics (same turn behaviour, etc)."
                    + "\n\n\"Same rank only\" strictly picks targets of the same rank."
                    + "\n\"One rank below\" also accepts targets one rank below."
                    + "\n\"Anything allowed\" picks any target it can find, regardless of the rank."
                    + "\n\nNone of them ever breaks your other rules."
                    + "\n\nRecommended: \"One rank below\", or \"Same rank only\" if you want behavior accuracy.")))
                {
                    Configuration.DispatchFidelity = (DispatchFidelity)dispatchFidelity;
                }

                var moddedTargets = (int)Configuration.ModdedTargets;
                if (ComboRow(ModdedTargetsName, "##BypassEmoteModdedTargets", ref moddedTargets, ModdedTargetsOptions,
                    L.T("Determines whether to block unlocked emotes from being picked when they are modified by at least one of your mods. "
                    + "This prevents other people from seeing other modded emotes you might have before the swap takes place."
                    + "\nAs an example, you have a mod on beesknees, and you try to bypass /conduct which happens to land on beesknees: "
                    + "other players might or might not see the modded beesknees for a moment."
                    + "\n\n\"Allowed\" allows using unlocked emotes that are modified by one of your mods."
                    + "\n\"Last resort\" uses one only when nothing else fits."
                    + "\n\"Blocked\" never uses one, and show an error message in the chat with options to disable or open the mod in penumbra."
                    + "\n\nRecommended: \"Last resort\", or \"Blocked\" if you absolutely don't want your modded emotes to accidentaly be seen.")))
                {
                    Configuration.ModdedTargets = (ModdedTargetRule)moddedTargets;
                }

                var idlePoseLoops = (int)Configuration.IdlePoseLoops;
                if (ComboRow(IdlePoseLoopsName, "##BypassEmoteIdlePoseLoops", ref idlePoseLoops, IdlePoseLoopsOptions,
                    L.T("When no unlocked looped emote fits as a target, your current idle pose may be eligible instead. "
                    + "The emote you try to bypass will then be targeted onto your current idle pose. This will cause a redraw of your character when triggered."
                    + "\n\n\"Never\" blocks idle poses from being used as targets."
                    + "\n\"Only when nothing else fits\" uses the pose only when literally no unlocked looped emote could have played here at all. "
                    + "This will not use your idle pose if any other emote would have been available if it wasn't blocked."
                    + "\n\"Allow\" always falls back to your idle pose when no other options are available."
                    + "\n\nRecommended: \"Only when nothing else fits\".")))
                {
                    Configuration.IdlePoseLoops = (IdlePoseFallback)idlePoseLoops;
                }
            }
        }

        SettingsLayout.Heading(L.T("Penumbra"));

        using (var rows = SettingsLayout.Rows("##BypassEmotePenumbraRows", names, controls))
        {
            if (rows)
            {
                var swapLifetime = (int)Configuration.SwapLifetime;
                if (ComboRow(LifetimeName, "##BypassEmoteLifetime", ref swapLifetime, SwapLifetimeOptions,
                    L.T("\"When the emote ends\" puts your real emote back as soon as the animation stops."
                    + "\n\"When you play the target emote\" keeps the swap enabled until the next time you play the target emote."
                    + "\n\"Never\" keeps the swap live until another swap claims the same target emote."
                    + "\n\nRecommended: \"When you play the target emote\". \"When the emote ends\" is not recommended, as people will "
                    + "see you redraw constantly after swapping.")))
                {
                    Configuration.SwapLifetime = (SwapLifetime)swapLifetime;
                }

                var swapBehavior = (int)Configuration.SwapBehavior;
                if (ComboRow(BehaviorName, "##BypassEmoteSwapBehavior", ref swapBehavior, SwapBehaviorOptions,
                    L.T("\"Multiple swaps\" keeps multiple swaps active at the same time in the mod."
                    + "\n\"One swap at a time\" turns the previous swaps off as soon as a new one starts."
                    + "\n\nRecommended: \"Multiple swaps\", unless for some reason you want to only keep one swap at a time.")))
                {
                    Configuration.SwapBehavior = (SwapBehavior)swapBehavior;
                }

                SettingsLayout.Name(KeptSwapsName);

                var maxKeptSwaps = Configuration.MaxKeptSwapsPerTarget;
                if (NoireInputs.Number("###BypassEmoteMaxKeptSwaps", ref maxKeptSwaps, KeptSwapsStyle))
                    Configuration.MaxKeptSwapsPerTarget = maxKeptSwaps;

                SettingsLayout.Help(L.T("How many swaps are kept per target emote. 0 keeps them all."
                    + "\n\nEach kept swap stays as an option of the generated Penumbra mod, so playing an emote "
                    + "again enables it again without rebuilding it. The only cost is that the mod gets bigger."));

                var anonymizeModName = Configuration.AnonymizeModName;
                if (CheckRow(AnonymizeModName, ref anonymizeModName,
                    L.T("Names the generated mod after your initials instead of your full name and world.")))
                {
                    Configuration.AnonymizeModName = anonymizeModName;
                }

                var alwaysCacheBreak = Configuration.AlwaysCacheBreak;
                if (CheckRow(AlwaysCacheBreakName, ref alwaysCacheBreak,
                    L.T("Applies the cache-break mechanism to every emote, even those you own. This is a bonus feature and "
                    + "is experimental. This will allow the client to always \"refresh\" animations so you never have "
                    + "to redraw yourself or stop emoting to apply an animation change."),
                    AlwaysCacheBreakAlarm()))
                {
                    Configuration.AlwaysCacheBreak = alwaysCacheBreak;
                }
            }
        }

        SettingsLayout.Heading(L.T("Chat messages"));

        using (var rows = SettingsLayout.Rows("##BypassEmoteMessageRows", names, controls))
        {
            if (rows)
            {
                var showSwapMessages = Configuration.ShowSwapMessages;
                if (CheckRow(SwapMessagesName, ref showSwapMessages, L.T("Shows a message in chat when an emote is swapped.")))
                    Configuration.ShowSwapMessages = showSwapMessages;

                var showErrorMessages = Configuration.ShowErrorMessages;
                if (CheckRow(ErrorMessagesName, ref showErrorMessages,
                    L.T("Shows an error in chat when a swap could not be done.")))
                {
                    Configuration.ShowErrorMessages = showErrorMessages;
                }

                SettingsLayout.Name(ErrorThrottleName);

                using (ImRaii.Disabled(!Configuration.ShowErrorMessages))
                {
                    var throttleTimeErrors = Configuration.ThrottleTimeErrors;
                    if (NoireInputs.Duration("###BypassEmoteErrorThrottle", ref throttleTimeErrors, ErrorThrottleStyle))
                        Configuration.ThrottleTimeErrors = throttleTimeErrors;
                }

                SettingsLayout.Help(L.T("How long the same error stays quiet after you have seen it."
                    + "\n\nAccepts time format: 5m, 300s, 5m10s, 1h."
                    + "\nSet to 0 to see every one of them."));

                var showWarningMessages = Configuration.ShowWarningMessages;
                if (CheckRow(WarningMessagesName, ref showWarningMessages,
                    L.T("Shows a warning in chat when a swap happened, but not the way you would expect.")))
                {
                    Configuration.ShowWarningMessages = showWarningMessages;
                }

                SettingsLayout.Name(WarningThrottleName);

                using (ImRaii.Disabled(!Configuration.ShowWarningMessages))
                {
                    var throttleTimeWarnings = Configuration.ThrottleTimeWarnings;
                    if (NoireInputs.Duration("###BypassEmoteWarningThrottle", ref throttleTimeWarnings, WarningThrottleStyle))
                        Configuration.ThrottleTimeWarnings = throttleTimeWarnings;
                }

                SettingsLayout.Help(L.T("How long the same warning stays quiet after you have seen it."
                    + "\n\nAccepts time format: 5m, 300s, 5m10s, 1h."
                    + "\nSet to 0 to disable."));
            }
        }
    }

    private static string? AlwaysCacheBreakAlarm()
    {
        if (Service.PatchApproval is { HoldsHooks: true })
            return L.T("This game build is not approved yet, this will not work.");

        return Service.Rebinder?.Fault is { } fault ? L.T("Not running: {0}.", fault) : null;
    }

    private static string? CachedDispatchAlarm()
    {
        var fault = Service.Rebinder?.Fault;
        var spreads = Configuration.CachedDispatch != CachedDispatchMode.Off;

        if (spreads && fault == null)
            return null;

        var message = L.T("Leave this on, otherwise bypassing several animations in a row may look broken.");

        if (fault == null)
            return message;

        return (spreads ? string.Empty : message + "\n\n")
            + L.T("Not running: {0}.", fault);
    }

    private static bool ComboRow(string name, string id, ref int index, string[] options, string help, string? alarm = null)
    {
        SettingsLayout.Name(name, alarm);
        var changed = ImGui.Combo(id, ref index, options, options.Length);
        SettingsLayout.Help(help);

        return changed;
    }

    private static bool CheckRow(string name, ref bool value, string help, string? alarm = null)
    {
        var changed = SettingsLayout.Check(name, ref value, alarm);
        SettingsLayout.Help(help);

        return changed;
    }

    private static NoireContent UnsafeWarningContent(bool withSyncLine)
    {
        var danger = NoireTheme.Current.Resolve(ThemeColor.Danger);

        var content = new NoireContent()
            .AddIcon(FontAwesomeIcon.ExclamationTriangle, danger)
            .AddSpacing(6f)
            .AddText(UnsafeHeadline, danger)
            .AddNewLine()
            .AddNewLine()
            .AddText(UnsafeReassurance);

        if (withSyncLine)
            content.AddNewLine().AddNewLine().AddText(SyncServicesLine);

        return content;
    }

    private static async Task ConfirmSwitchToDirectPlayAsync()
    {
        var liftsTheLimit = Configuration.DirectPlayUnsafe;

        var message = liftsTheLimit
            ? UnsafeWarningContent(true)
            : new NoireContent()
                .AddText(SyncServicesLine)
                .AddNewLine()
                .AddNewLine()
                .AddText(SafeModeLimitLine)
                .AddNewLine()
                .AddNewLine()
                .AddText(SafeModeIsNotAPromiseLine, NoireTheme.Current.Resolve(ThemeColor.TextMuted));

        var confirmed = await NoireModal.ConfirmAsync(L.T("Switch to Direct Play?"), message, new ModalOptions
        {
            ConfirmLabel = L.T("Switch to Direct Play"),
            CancelLabel = L.T("Cancel"),
            Danger = liftsTheLimit,
            EnableAfterSeconds = liftsTheLimit ? WarningCountdownSeconds : 0f,
        });

        if (confirmed)
            ModeSwitcher.Apply(SelfBypassMode.DirectPlay);
    }

    private static async Task ConfirmEnableUnsafeAsync()
    {
        if (confirmingUnsafe)
            return;

        confirmingUnsafe = true;

        try
        {
            var confirmed = await NoireModal.ConfirmAsync(L.T("Enable unsafe mode?"), UnsafeWarningContent(false), new ModalOptions
            {
                ConfirmLabel = L.T("Enable unsafe mode"),
                CancelLabel = L.T("Cancel"),
                Danger = true,
                EnableAfterSeconds = WarningCountdownSeconds,
            });

            if (!confirmed)
                return;

            await AsyncHelper.RunOnFrameworkThreadAsync(() => Configuration.DirectPlayUnsafe = true);
        }
        finally
        {
            confirmingUnsafe = false;
        }
    }

    public void Dispose() { }
}
