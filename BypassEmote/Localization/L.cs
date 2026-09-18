using BypassEmote.Helpers;
using Dalamud.Game;
using NoireLib;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BypassEmote.Localization;

/// <summary>
/// Plugin wide localization.
/// <para/>
/// Every user visible string goes through <see cref="T(string)"/> and uses its own English text as the key,
/// so no call site has to keep a separate key in sync with the text. Anything without a registered
/// translation simply falls back to English, which keeps a partial translation readable instead of blank.
/// </summary>
public static class L
{
    private static readonly Dictionary<string, string> Table = new(StringComparer.Ordinal);

    private static bool clientIsChinese;

    static L()
    {
        ZhTables.Register(Table);
    }

    /// <summary> Language picked in the configuration. </summary>
    public static PluginLanguage Language { get; private set; } = PluginLanguage.Auto;

    /// <summary> Whether Chinese is currently being displayed. </summary>
    public static bool IsChinese { get; private set; }

    /// <summary> How many Chinese strings are registered. Diagnostic only. </summary>
    public static int TranslationCount => Table.Count;

    /// <summary> Switches the language. Safe to call at any time: the UI reads it every frame. </summary>
    public static void Apply(PluginLanguage language)
    {
        Language = language;

        IsChinese = language switch
        {
            PluginLanguage.ChineseSimplified => true,
            PluginLanguage.English => false,
            _ => clientIsChinese,
        };
    }

    /// <summary> Resolves <see cref="PluginLanguage.Auto"/> once, from the running game client. </summary>
    public static void DetectClientLanguage()
    {
        var uiLanguage = Plugin.PluginInterface.UiLanguage ?? string.Empty;

        ClientLanguage clientLanguage;

        try
        {
            clientLanguage = NoireService.ClientState.ClientLanguage;
        }
        catch (Exception ex)
        {
            Log.Warning($"[L10n] Could not read the client language: {ex.Message}");
            clientLanguage = ClientLanguage.English;
        }

        var clientChinese = clientLanguage is ClientLanguage.ChineseSimplified
            or ClientLanguage.ChineseTraditional
            or ClientLanguage.TraditionalChinese;

        clientIsChinese = clientChinese || uiLanguage.StartsWith("zh", StringComparison.OrdinalIgnoreCase);

        Apply(Configuration.Language);

        Log.Info($"[L10n] UiLanguage={uiLanguage}, ClientLanguage={clientLanguage}, "
            + $"chinese={clientIsChinese}, language={Language}, strings={Table.Count}");
    }

    /// <summary> Translates an English string. Falls back to English when there is no translation. </summary>
    /// <remarks>
    /// Accepts null so a call site can pass an optional value straight through; a null source becomes an empty
    /// string, which is what an empty fallback would have produced anyway.
    /// </remarks>
    public static string T(string? english) =>
        IsChinese && !string.IsNullOrEmpty(english) ? Text(english) : english ?? string.Empty;

    /// <summary>
    /// Translates a format template and fills it in.
    /// Prefer this over interpolating first, otherwise the values would be part of the lookup key.
    /// </summary>
    public static string T(string english, params object?[] args) =>
        string.Format(CultureInfo.InvariantCulture, T(english), args);

    private static string Text(string english) =>
        !string.IsNullOrEmpty(english) && Table.TryGetValue(english, out var chinese) ? chinese : english;
}
