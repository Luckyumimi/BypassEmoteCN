namespace BypassEmote.Localization;

internal static class L
{
    internal static string T(string? text) => text ?? string.Empty;
    internal static string T(string text, params object?[] args) => string.Format(text, args);
}
