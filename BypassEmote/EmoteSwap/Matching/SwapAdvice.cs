using BypassEmote.Enums;
using BypassEmote.Models;
using System.Collections.Generic;
using System.Linq;

namespace BypassEmote.EmoteSwap;

public static class SwapAdvice
{
    public enum Severity
    {
        Note,
        Warning,
        Error,
    }

    public readonly record struct Line(Severity Severity, string Text);

    public sealed record Facts(bool Unlocked, bool Blocked, string? ChangedByMod);

    public static Severity WorstOf(IReadOnlyList<Line> lines)
    {
        var worst = Severity.Note;

        foreach (var line in lines)
        {
            if (line.Severity > worst)
                worst = line.Severity;
        }

        return worst;
    }

    public static IReadOnlyList<Line> ForOverride(EmoteAttributes source, string sourceName, EmoteAttributes target,
        string targetName, Facts facts)
    {
        if (source.RowId == target.RowId)
            return [new Line(Severity.Error, L.T("{0} is the emote being bypassed.", targetName))];

        var lines = new List<Line>();

        if (!target.EligibleTarget)
        {
            lines.Add(new Line(Severity.Error, L.T("{0} can never be a swap target: it is a pose, a facial "
                + "expression, a per-job emote, or it draws your weapon. It is always skipped.", targetName)));
        }

        if (!facts.Unlocked)
        {
            lines.Add(new Line(Severity.Note, L.T("You have not unlocked {0}. It is skipped.", targetName)));
        }

        if (facts.Blocked)
        {
            lines.Add(new Line(Severity.Warning,
                L.T("{0} is on your blocked targets list. This override uses it anyway.", targetName)));
        }

        if (facts.ChangedByMod is { Length: > 0 } modName)
        {
            lines.Add(new Line(Severity.Warning, L.T("Your mod \"{0}\" already changes {1}. What your "
                + "\"Emotes your mods change\" setting is set to still applies here.", modName, targetName)));
        }

        lines.AddRange(Behaviour(source, sourceName, target, targetName));

        return lines;
    }

    public static IReadOnlyList<Line> Behaviour(EmoteAttributes source, string sourceName, EmoteAttributes target,
        string targetName)
    {
        var lines = new List<Line>();

        if ((source.Postures & target.Postures) == PostureFlags.None)
        {
            lines.Add(new Line(Severity.Error,
                L.T("{0} and {1} share no posture. The swap will not happen.", targetName, sourceName)));
        }
        else if ((source.Postures & ~target.Postures) is var missing && missing != PostureFlags.None)
        {
            lines.Add(new Line(Severity.Warning, L.T("{0} has no {1} animation. The swap "
                + "will not happen when you play {2} in that posture.", targetName, PostureText(missing), sourceName)));
        }

        AddLoopLine(lines, source, sourceName, target, targetName);
        AddTurnLine(lines, source, sourceName, target, targetName);
        AddSoundLine(lines, source, sourceName, target, targetName);
        AddIntroLine(lines, source, sourceName, target, targetName);
        AddAdjustLine(lines, source, sourceName, target, targetName);

        if (target.CancelsOnRotate)
        {
            lines.Add(new Line(Severity.Warning, L.T("{0} will stop when you turn your character.", targetName)));
        }

        return lines;
    }

    private static void AddAdjustLine(List<Line> lines, EmoteAttributes source, string sourceName,
        EmoteAttributes target, string targetName)
    {
        if (target.AdjustRelativePapPath == null)
            return;

        if (source.AdjustRelativePapPath != null
            || source.Variants.Any(variant => variant.Posture == PostureFlags.Mounted))
        {
            return;
        }

        lines.Add(new Line(Severity.Warning, L.T("{0} plays a separate animation when you use it on someone (adjust variant) "
            + "but {1} has no adjust variant. Targeting someone will keep {2}'s animation.",
            targetName, sourceName, targetName)));
    }

    private static void AddLoopLine(List<Line> lines, EmoteAttributes source, string sourceName,
        EmoteAttributes target, string targetName)
    {
        if (source.LoopKind == target.LoopKind)
            return;

        lines.Add(source.LoopKind == EmotePlayType.Looped
            ? new Line(Severity.Warning, L.T("{0} plays once while {1} loops. The animation will stop "
                + "instead of looping.", targetName, sourceName))
            : new Line(Severity.Warning, L.T("{0} loops while {1} plays once. Weird behavior might happen.",
                targetName, sourceName)));
    }

    private static void AddTurnLine(List<Line> lines, EmoteAttributes source, string sourceName,
        EmoteAttributes target, string targetName)
    {
        if (source.Turn == target.Turn)
            return;

        lines.Add(new Line(Severity.Warning, L.T("{0} does not behave like {1} when you target "
            + "someone: {0} {2} while {1} {3}.", targetName, sourceName, TurnText(target.Turn),
            TurnText(source.Turn))));
    }

    private static void AddSoundLine(List<Line> lines, EmoteAttributes source, string sourceName,
        EmoteAttributes target, string targetName)
    {
        if (target.Sound == SoundClass.Voiceline && source.Sound != SoundClass.Voiceline)
        {
            lines.Add(new Line(Severity.Warning,
                L.T("{0} emits a voice line sound. Everyone around you will hear it.", targetName)));
            return;
        }

        if (target.Sound == SoundClass.Sfx && source.Sound == SoundClass.Silent)
        {
            lines.Add(new Line(Severity.Warning,
                L.T("{0} emits a sound that {1} does not. Everyone around you will hear it.", targetName, sourceName)));
        }
    }

    private static void AddIntroLine(List<Line> lines, EmoteAttributes source, string sourceName,
        EmoteAttributes target, string targetName)
    {
        var sourceHasIntro = source.Intro == IntroKind.Pap;

        if (sourceHasIntro && target.Intro == IntroKind.None || target.Intro == IntroKind.TmbOnly)
        {
            lines.Add(new Line(Severity.Warning,
                L.T("{0} has no intro meanwhile {1} has one, meaning the intro will not play.", targetName, sourceName)));
            return;
        }

        if (!sourceHasIntro && target.Intro == IntroKind.Pap)
        {
            lines.Add(new Line(Severity.Warning, L.T("{0} has an intro and {1} does not.", targetName, sourceName)));
        }
    }

    private static string TurnText(TurnClass turn) => turn switch
    {
        TurnClass.None => L.T("does not turn at all"),
        TurnClass.Eyes => L.T("only follows with the eyes"),
        TurnClass.Head => L.T("turns the head"),
        TurnClass.Body => L.T("turns the whole body"),
        _ => L.T("turns in a way the plugin could not read"),
    };

    private static string PostureText(PostureFlags postures)
    {
        var names = new List<string>(4);

        if (postures.HasFlag(PostureFlags.Standing))
            names.Add(L.T("standing"));

        if (postures.HasFlag(PostureFlags.ChairSit))
            names.Add(L.T("chair sitting"));

        if (postures.HasFlag(PostureFlags.GroundSit))
            names.Add(L.T("ground sitting"));

        if (postures.HasFlag(PostureFlags.Mounted))
            names.Add(L.T("mounted"));

        return names.Count == 0 ? L.T("matching") : string.Join(L.T(" or "), names);
    }
}
