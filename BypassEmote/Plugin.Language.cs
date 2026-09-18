using BypassEmote.Helpers;
using System;

namespace BypassEmote;

public sealed partial class Plugin
{
    /// <summary> Handles <c>/be lang &lt;auto|zh|en&gt;</c>. </summary>
    private static void SetLanguage(string? value)
    {
        var token = (value ?? string.Empty).Trim().ToLowerInvariant();

        PluginLanguage? language = token switch
        {
            "" or "auto" or "a" => PluginLanguage.Auto,
            "zh" or "cn" or "chs" or "zh-cn" or "chinese" => PluginLanguage.ChineseSimplified,
            "en" or "eng" or "english" => PluginLanguage.English,
            _ => null,
        };

        if (language is null)
        {
            LogHelper.Info(L.T("Unknown language \"{0}\". Usage: /be lang <auto|zh|en>", value ?? string.Empty));
            return;
        }

        Configuration.Language = language.Value;
        L.Apply(language.Value);

        LogHelper.Success(L.T("Plugin language: {0}.", LanguageDisplayName(language.Value)));
    }

    private static string LanguageDisplayName(PluginLanguage language) => language switch
    {
        PluginLanguage.ChineseSimplified => "简体中文",
        PluginLanguage.English => "English",
        _ => L.T("Follow the game"),
    };
}
