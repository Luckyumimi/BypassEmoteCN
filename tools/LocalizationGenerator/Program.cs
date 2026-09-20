using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length != 3)
    throw new ArgumentException("Usage: LocalizationGenerator <source> <translations.tsv> <output>");

var sourceRoot = Path.GetFullPath(args[0]);
var tablePath = Path.GetFullPath(args[1]);
var outputRoot = Path.GetFullPath(args[2]);
var translations = LoadTranslations(tablePath);

if (Directory.Exists(outputRoot))
    Directory.Delete(outputRoot, true);
Directory.CreateDirectory(outputRoot);

var wrapped = 0;
foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
             .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                 && !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                 && !string.Equals(Path.GetFileName(path), "L.cs", StringComparison.OrdinalIgnoreCase)))
{
    var relative = Path.GetRelativePath(sourceRoot, file);
    var destination = Path.Combine(outputRoot, relative);
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
    var rewriter = new TranslationRewriter(translations);
    var root = rewriter.Visit(tree.GetRoot())!;
    wrapped += rewriter.Count;
    File.WriteAllText(destination, root.ToFullString(), new UTF8Encoding(false));
}

File.WriteAllText(Path.Combine(outputRoot, "Localization.Generated.cs"), GenerateTable(translations), new UTF8Encoding(false));
Console.WriteLine($"Generated {translations.Count} translations and wrapped {wrapped} source expressions.");
if (wrapped == 0)
    throw new InvalidOperationException("No source expressions were localized; refusing to build an untranslated package.");

static Dictionary<string, string> LoadTranslations(string path)
{
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    var placeholders = new Regex(@"\{\d+(?:[^}]*)\}", RegexOptions.Compiled);
    foreach (var line in File.ReadLines(path))
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            continue;
        var split = line.IndexOf('\t');
        if (split <= 0)
            throw new InvalidDataException($"Malformed translation row: {line}");
        var english = Unescape(line[..split]);
        var chinese = Unescape(line[(split + 1)..]);
        if (string.IsNullOrWhiteSpace(chinese))
            throw new InvalidDataException($"Empty translation: {english}");
        var sourceArgs = placeholders.Matches(english).Select(m => m.Value).ToHashSet(StringComparer.Ordinal);
        var targetArgs = placeholders.Matches(chinese).Select(m => m.Value).ToHashSet(StringComparer.Ordinal);
        if (!sourceArgs.SetEquals(targetArgs))
            throw new InvalidDataException($"Placeholder mismatch: {english}");
        if (!result.TryAdd(english, chinese) && result[english] != chinese)
            throw new InvalidDataException($"Conflicting translation: {english}");
    }
    return result;
}

// translations.tsv escapes a literal double quote as \" the same way it escapes \n and \t.
// Without this replacement every key containing a quote keeps its backslashes, never matches
// the C# literal at the call site, and silently falls back to English.
static string Unescape(string value) => value
    .Replace("\\r", "\r")
    .Replace("\\n", "\n")
    .Replace("\\t", "\t")
    .Replace("\\\"", "\"");

static string GenerateTable(Dictionary<string, string> translations)
{
    var rows = string.Join(",\n", translations.Select(pair =>
        $"        [{SymbolDisplay.FormatLiteral(pair.Key, true)}] = {SymbolDisplay.FormatLiteral(pair.Value, true)}"));
    return $$"""
#nullable enable
global using BypassEmote.Localization;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace BypassEmote.Localization;

internal static class L
{
    private static readonly Dictionary<string, string> Table = new(StringComparer.Ordinal)
    {
{{rows}}
    };

    internal static string T(string? english) =>
        !string.IsNullOrEmpty(english) && Table.TryGetValue(english, out var chinese) ? chinese : english ?? string.Empty;

    internal static string T(string english, params object?[] args) =>
        string.Format(CultureInfo.InvariantCulture, T(english), args);
}
""";
}

sealed class TranslationRewriter(IReadOnlyDictionary<string, string> translations) : CSharpSyntaxRewriter
{
    public int Count { get; private set; }

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node) => base.VisitExpressionStatement(node);

    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (IsLocalizationCall(node))
            return node;
        return base.VisitInvocationExpression(node);
    }

    public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node) => WrapIfEligible(node);

    public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        if (node.Ancestors().OfType<InvocationExpressionSyntax>().Any(IsLocalizationCall))
            return base.VisitBinaryExpression(node);
        if (node.IsKind(SyntaxKind.AddExpression) && TryConstant(node, out var text)
            && translations.ContainsKey(text) && !IsForbidden(node))
            return Wrap(node);
        return base.VisitBinaryExpression(node);
    }

    private SyntaxNode WrapIfEligible(LiteralExpressionSyntax node)
    {
        if (!node.IsKind(SyntaxKind.StringLiteralExpression) || node.Token.ValueText is not { } text
            || !translations.ContainsKey(text) || IsForbidden(node))
            return base.VisitLiteralExpression(node)!;
        return Wrap(node);
    }

    private ExpressionSyntax Wrap(ExpressionSyntax expression)
    {
        if (expression.Ancestors().OfType<InvocationExpressionSyntax>().Any(IsLocalizationCall))
            return expression;
        Count++;
        return SyntaxFactory.InvocationExpression(SyntaxFactory.ParseExpression("L.T"),
                SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(expression.WithoutTrivia()))))
            .WithTriviaFrom(expression);
    }

    private static bool IsForbidden(SyntaxNode node) => node.Ancestors().OfType<InvocationExpressionSyntax>().Any(IsLocalizationCall)
        || node.Ancestors().Any(parent => parent is AttributeSyntax
        or CaseSwitchLabelSyntax or ConstantPatternSyntax or ParameterSyntax
        || parent is FieldDeclarationSyntax field && field.Modifiers.Any(SyntaxKind.ConstKeyword)
        || parent is LocalDeclarationStatementSyntax local && local.Modifiers.Any(SyntaxKind.ConstKeyword));

    private static bool IsLocalizationCall(InvocationExpressionSyntax invocation)
        => invocation.Expression.ToString() is "L.T" or "SettingsLayout.Help";

    private static bool TryConstant(ExpressionSyntax expression, out string value)
    {
        if (expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            value = literal.Token.ValueText;
            return true;
        }
        if (expression is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.AddExpression)
            && TryConstant(binary.Left, out var left) && TryConstant(binary.Right, out var right))
        {
            value = left + right;
            return true;
        }
        value = string.Empty;
        return false;
    }

}
