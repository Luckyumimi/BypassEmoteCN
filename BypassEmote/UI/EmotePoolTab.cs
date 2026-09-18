#if DEBUG
using BypassEmote.EmoteSwap;
using BypassEmote.Enums;
using BypassEmote.Helpers;
using BypassEmote.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;
using NoireLib;
using NoireLib.Enums;
using NoireLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace BypassEmote.UI;

internal static class EmotePoolTab
{
    private static uint _sourceEmoteId;
    private static string _sourceSearch = string.Empty;
    private static string _targetSearch = string.Empty;
    private static EmoteCondition _condition = EmoteCondition.Standing;
    private static bool _conditionPicked;
    private static bool _showRefused = true;
    private static bool _showExcluded = true;
    private static List<Emote>? _emotes;

    private static readonly Vector4 Chosen = new(0.45f, 0.90f, 0.55f, 1f);
    private static readonly Vector4 SameTier = new(0.55f, 0.75f, 1.00f, 1f);
    private static readonly Vector4 Refused = new(0.65f, 0.65f, 0.65f, 1f);
    private static readonly Vector4 Excluded = new(0.50f, 0.50f, 0.50f, 1f);
    private static readonly Vector4 TagColor = new(0.80f, 0.72f, 0.45f, 1f);
    private static readonly Vector4 Usable = new(0.88f, 0.88f, 0.88f, 1f);
    private static readonly Vector4 Notice = new(1.00f, 0.65f, 0.20f, 1f);
    private static readonly Vector4 Blocked = new(0.95f, 0.35f, 0.35f, 1f);

    private static readonly EmoteCondition[] Conditions =
    [
        EmoteCondition.Standing,
        EmoteCondition.SittingInChair,
        EmoteCondition.SittingOnGround,
        EmoteCondition.Mounted,
        EmoteCondition.Swimming,
        EmoteCondition.Diving,
        EmoteCondition.Fishing,
        EmoteCondition.HoldingUmbrella,
        EmoteCondition.HoldingTorch,
        EmoteCondition.WearingFashionAccessory,
    ];

    /// <summary> Label shown for a condition. The enum name is also the English label, so the key stays stable. </summary>
    private static string ConditionLabel(EmoteCondition condition) => condition switch
    {
        EmoteCondition.Standing => L.T("Standing"),
        EmoteCondition.SittingInChair => L.T("SittingInChair"),
        EmoteCondition.SittingOnGround => L.T("SittingOnGround"),
        EmoteCondition.Mounted => L.T("Mounted"),
        EmoteCondition.Swimming => L.T("Swimming"),
        EmoteCondition.Diving => L.T("Diving"),
        EmoteCondition.Fishing => L.T("Fishing"),
        EmoteCondition.HoldingUmbrella => L.T("HoldingUmbrella"),
        EmoteCondition.HoldingTorch => L.T("HoldingTorch"),
        EmoteCondition.WearingFashionAccessory => L.T("WearingFashionAccessory"),
        _ => condition.ToString(),
    };

    private sealed record PoolView(
        SwapOrchestrator.SwapPreview Preview,
        List<EmoteAttributes> Accepted,
        List<(EmoteAttributes Candidate, string BlockedBy)> Refused,
        HashSet<uint> TierIds);

    private static PoolView? _view;
    private static string _viewSignature = string.Empty;

    private static HashSet<uint>? _unplayableSources;
    private static EmoteCondition _unplayableFor;

    internal static void Draw()
    {
        var catalog = Service.Catalog;

        if (catalog == null || !catalog.Ready)
        {
            ImGui.TextWrapped(L.T("The emote catalog is still building."));
            return;
        }

        DrawControls();
        ImGui.Separator();

        var listWidth = MathF.Max(220f, ImGui.GetContentRegionAvail().X * 0.32f);

        using (ImRaii.Group())
        {
            ImGui.SetNextItemWidth(listWidth);
            ImGui.InputTextWithHint("##EmotePoolSourceSearch", L.T("Search emotes..."), ref _sourceSearch, 256);

            using var list = ImRaii.Child("EmotePoolSources", new Vector2(listWidth, 0), true);

            if (list)
                DrawSourceList();
        }

        ImGui.SameLine();

        using var panel = ImRaii.Child("EmotePoolTargets", Vector2.Zero, true);
        if (panel)
            DrawPool();
    }

    private static void DrawControls()
    {
        if (!_conditionPicked && NoireService.ObjectTable.LocalPlayer is { } player)
        {
            _condition = EmoteHelper.ConditionOf(player);
            _conditionPicked = true;
        }

        ImGui.SetNextItemWidth(200);

        using (var combo = ImRaii.Combo(L.T("State") + "##EmotePool", ConditionLabel(_condition)))
        {
            if (combo)
            {
                foreach (var condition in Conditions)
                {
                    if (ImGui.Selectable(ConditionLabel(condition), _condition == condition))
                    {
                        _condition = condition;
                        _conditionPicked = true;
                    }
                }
            }
        }

        ImGui.SameLine();

        if (ImGui.Button(L.T("Read my state") + "##EmotePool") && NoireService.ObjectTable.LocalPlayer is { } current)
        {
            _condition = EmoteHelper.ConditionOf(current);
            _conditionPicked = true;
        }

        ImGui.SameLine();

        if (ImGui.Button(L.T("Refresh") + "##EmotePool"))
            _viewSignature = string.Empty;

        ImGui.SameLine();
        ImGui.Checkbox(L.T("Refused") + "##EmotePool", ref _showRefused);

        ImGui.SameLine();
        ImGui.Checkbox(L.T("Kept out") + "##EmotePool", ref _showExcluded);

        ImGui.TextDisabled(L.T("rules: loop {0}, turn {1}, sound {2}, modded {3}, idle pose {4}, dispatch {5}",
            Configuration.LoopMatching, Configuration.TurnMatching, Configuration.SoundMatching,
            Configuration.ModdedTargets, Configuration.IdlePoseLoops,
            Configuration.CachedDispatch == CachedDispatchMode.Off
                ? L.T("off")
                : L.T("{0}, {1} per rank, {2}", Configuration.CachedDispatch,
                    Configuration.MaxTargetsPerRank, Configuration.DispatchFidelity)));
    }

    private static void DrawSourceList()
    {
        var unplayable = UnplayableSources();

        foreach (var emote in Emotes())
        {
            var name = CommonHelper.GetEmoteName(emote);

            if (!string.IsNullOrWhiteSpace(_sourceSearch)
                && !name.Contains(_sourceSearch, StringComparison.OrdinalIgnoreCase))
                continue;

            var refusedHere = unplayable.Contains(emote.RowId);

            using (ImRaii.PushColor(ImGuiCol.Text, Excluded, refusedHere))
            {
                if (ImGui.Selectable($"{name}{(refusedHere ? "  " + L.T("(not here)") : string.Empty)}##src{emote.RowId}",
                    _sourceEmoteId == emote.RowId))
                {
                    _sourceEmoteId = emote.RowId;
                }
            }
        }
    }

    private static HashSet<uint> UnplayableSources()
    {
        if (_unplayableSources != null && _unplayableFor == _condition)
            return _unplayableSources;

        var condition = DirectPlayPlanner.PlayableAsFor(_condition);
        var unplayable = new HashSet<uint>();

        foreach (var emote in Emotes())
        {
            if ((EmoteHelper.GetEmoteConditions(emote) & condition) != condition)
                unplayable.Add(emote.RowId);
        }

        _unplayableFor = _condition;
        _unplayableSources = unplayable;
        return unplayable;
    }

    private static void DrawPool()
    {
        if (_sourceEmoteId == 0)
        {
            ImGui.TextDisabled(L.T("No source emote selected."));
            return;
        }

        if (Service.Orchestrator is not { } orchestrator)
        {
            ImGui.TextColored(Blocked, L.T("Emote Swap is not running, so there is nothing to preview."));
            return;
        }

        var view = ViewFor(orchestrator);

        if (view == null)
        {
            ImGui.TextDisabled(L.T("Nothing to show."));
            return;
        }

        DrawSourceVerdict(view.Preview);

        if (view.Preview.Source == null || view.Preview.HandedToGame || view.Preview.Refusal != null)
            return;

        ImGui.Separator();
        DrawMatchVerdict(view);

        ImGui.Spacing();
        ImGui.SetNextItemWidth(-1);
        ImGui.InputTextWithHint("##EmotePoolTargetSearch", L.T("Filter targets..."), ref _targetSearch, 256);
        ImGui.Separator();

        var best = view.Preview.Match?.Target;

        foreach (var candidate in view.Preview.Tier)
            DrawCandidate(view, candidate, candidate.RowId == best?.RowId, null, null);

        foreach (var candidate in view.Accepted.Where(entry => !view.TierIds.Contains(entry.RowId))
                                               .OrderBy(entry => entry.RowId))
            DrawCandidate(view, candidate, false, null, null);

        if (_showRefused)
        {
            foreach (var (candidate, blockedBy) in view.Refused.OrderBy(entry => entry.Candidate.RowId))
                DrawCandidate(view, candidate, false, blockedBy, null);
        }

        if (!_showExcluded)
            return;

        foreach (var (candidate, reason) in view.Preview.Excluded.OrderBy(entry => entry.Candidate.RowId))
            DrawCandidate(view, candidate, false, null, reason);
    }

    private static void DrawSourceVerdict(SwapOrchestrator.SwapPreview preview)
    {
        var header = preview.Source is { } source
            ? $"{NameOf(source.RowId)}  /{source.Command}"
            : NameOf(_sourceEmoteId);

        if (preview.PipelineBlocked is { } stopped)
            ImGui.TextColored(Notice, stopped);

        ImGui.TextWrapped(header);

        if (preview.ResolvedFrom is { } pressed)
        {
            ImGui.SameLine();
            ImGui.TextColored(Notice, L.T(", you pressed {0}", NameOf(pressed)));
        }

        if (preview.Source is { } attributes)
            DrawTags(attributes);

        if (preview.HandedToGame)
        {
            ImGui.TextColored(Notice, L.T("Pose family, never swapped."));
            return;
        }

        if (preview.Refusal is { } refusal)
        {
            ImGui.TextColored(Blocked, L.T("Not playable here: {0}", refusal));
            return;
        }

        ImGui.TextDisabled(L.T("state {0}, channel {1}, body {2}",
            preview.Condition, preview.Posture, preview.Skeleton));

        if (!preview.GameGateApplied)
        {
            ImGui.TextColored(Notice, L.T("The condition check was skipped."));
        }
    }

    private static void DrawMatchVerdict(PoolView view)
    {
        var preview = view.Preview;
        var owned = preview.Pool.Count + preview.Excluded.Count;

        ImGui.TextWrapped(L.T("Pool of {0}: {1} usable, {2} refused. {3} of your {4} owned emotes were excluded.",
            preview.Pool.Count, view.Accepted.Count, view.Refused.Count, preview.Excluded.Count, owned));

        if (preview.TriesIdlePose)
        {
            ImGui.TextColored(Notice,
                L.T("Borrows your idle pose first. The match below is only used if that fails."));
        }

        if (preview.LoopsFirstFailed)
        {
            ImGui.TextColored(Notice, L.T("No owned loop fitted. This is the lenient pass, where a one-shot "
                + "can be used for a loop."));
        }

        if (preview.Match?.Target == null)
        {
            ImGui.TextColored(Blocked, L.T("Nothing passes the filters, so this emote cannot be bypassed here."));
            return;
        }

        if (preview.NoUsablePair)
        {
            ImGui.TextColored(Blocked, L.T("No posture variant shared with {0}. The swap stops "
                + "before it is built.", preview.Skeleton));
        }

        if (preview.WouldAlternate)
        {
            ImGui.TextColored(Notice, L.T("If this was to alternate, it would land on a second same-tier emote."));
        }
    }

    private static void DrawCandidate(PoolView view, EmoteAttributes candidate, bool isBest,
        string? blockedBy, string? keptOutBy)
    {
        var name = NameOf(candidate.RowId);

        if (!string.IsNullOrWhiteSpace(_targetSearch)
            && !name.Contains(_targetSearch, StringComparison.OrdinalIgnoreCase)
            && !candidate.Command.Contains(_targetSearch, StringComparison.OrdinalIgnoreCase))
            return;

        var colour = keptOutBy != null ? Excluded
            : blockedBy != null ? Refused
            : isBest ? Chosen
            : view.TierIds.Contains(candidate.RowId) ? SameTier
            : Usable;

        var prefix = keptOutBy != null ? L.T("kept out")
            : blockedBy != null ? L.T("refused")
            : isBest ? L.T("chosen")
            : view.TierIds.Contains(candidate.RowId) ? L.T("same tier")
            : L.T("usable");

        ImGui.TextColored(colour, $"[{prefix}] {name}  /{candidate.Command}");

        if (keptOutBy != null)
        {
            ImGui.SameLine();
            ImGui.TextColored(Excluded, $"- {L.T(keptOutBy.ToLowerInvariant())}");
        }
        else if (blockedBy != null)
        {
            ImGui.SameLine();

            ImGui.TextColored(Refused, blockedBy switch
            {
                BestMatchResolver.BlockedByRules => L.T(", blocked by your rules"),
                BestMatchResolver.BlockedByModdedTarget => L.T(", another of your mods changes it"),
                _ => L.T(", blocked on {0}", L.T(blockedBy.ToLowerInvariant())),
            });
        }

        ImGui.Indent();
        DrawTags(candidate);
        ImGui.Unindent();
    }

    private static void DrawTags(EmoteAttributes attributes)
    {
        var tags = new List<string>
        {
            ShapeTag(attributes),
            SoundTag(attributes.Sound),
            TurnTag(attributes.Turn),
            PostureTag(attributes.Postures),
        };

        if (attributes.CancelsOnRotate)
            tags.Add(L.T("cancels on rotate"));

        if (attributes.IsPoseFamily)
            tags.Add(L.T("pose family"));

        if (!attributes.EligibleTarget)
            tags.Add(L.T("never a target"));

        if (Configuration.BlockedTargetEmotesEmoteSwap.Contains(attributes.RowId))
            tags.Add(L.T("blocked by your rules"));

        ImGui.TextColored(TagColor, string.Join("  |  ", tags));
    }

    private static string ShapeTag(EmoteAttributes attributes)
    {
        var loops = attributes.LoopKind == EmotePlayType.Looped;

        return attributes.Intro == IntroKind.Pap
            ? loops ? L.T("intro + loop") : L.T("intro + one shot")
            : loops ? L.T("loop only") : L.T("one shot");
    }

    private static string SoundTag(SoundClass sound) => sound switch
    {
        SoundClass.Silent => L.T("no sound"),
        SoundClass.Sfx => L.T("sound"),
        SoundClass.Voiceline => L.T("voiceline"),
        _ => L.T("sound unknown"),
    };

    private static string TurnTag(TurnClass turn) => turn switch
    {
        TurnClass.None => L.T("no turn"),
        TurnClass.Eyes => L.T("eyes turn"),
        TurnClass.Head => L.T("head turn"),
        TurnClass.Body => L.T("body turn"),
        _ => L.T("turn unknown"),
    };

    private static string PostureTag(PostureFlags postures)
    {
        if (postures == PostureFlags.None)
            return L.T("no posture");

        var names = new List<string>();

        if (postures.HasFlag(PostureFlags.Standing))
            names.Add(L.T("standing"));

        if (postures.HasFlag(PostureFlags.ChairSit))
            names.Add(L.T("chair"));

        if (postures.HasFlag(PostureFlags.GroundSit))
            names.Add(L.T("ground"));

        if (postures.HasFlag(PostureFlags.Mounted))
            names.Add(L.T("mounted"));

        return string.Join(" ", names);
    }

    private static string NameOf(uint emoteRowId)
        => EmoteHelper.GetEmoteById(emoteRowId) is { } emote
            ? CommonHelper.GetEmoteName(emote)
            : L.T("Unknown ({0})", emoteRowId);

    private static PoolView? ViewFor(SwapOrchestrator orchestrator)
    {
        var signature = Signature();

        if (_view != null && _viewSignature == signature)
            return _view;

        var preview = orchestrator.Preview(_sourceEmoteId, _condition);

        var accepted = new List<EmoteAttributes>();
        var refused = new List<(EmoteAttributes Candidate, string BlockedBy)>();

        if (preview.Source is { } source && preview.Config is { } config)
        {
            foreach (var candidate in preview.Pool)
            {
                var single = BestMatchResolver.Resolve(source, [candidate], config, preview.Posture);

                if (single.Target != null)
                    accepted.Add(candidate);
                else if (single.Diagnostic is { } miss)
                    refused.Add((candidate, miss.BlockedBy));
            }
        }

        _view = new PoolView(preview, accepted, refused, preview.Tier.Select(entry => entry.RowId).ToHashSet());
        _viewSignature = signature;
        return _view;
    }

    private static string Signature()
    {
        var blocked = Configuration.BlockedTargetEmotesEmoteSwap;
        var live = NoireService.ObjectTable.LocalPlayer is { } player
            ? $"{EmoteHelper.ConditionOf(player)}/{SwapOrchestrator.SkeletonFor(player)}"
            : "none";

        return $"{_sourceEmoteId}|{_condition}|{live}|{Configuration.SelfBypassMode}|{Configuration.LoopMatching}"
            + $"|{Configuration.TurnMatching}"
            + $"|{Configuration.SoundMatching}|{Configuration.ModdedTargets}|{Configuration.IdlePoseLoops}"
            + $"|{Configuration.CachedDispatch}|{Configuration.MaxTargetsPerRank}|{Configuration.DispatchFidelity}|{blocked.Count}:{blocked.Sum(rowId => (long)rowId)}";
    }

    private static List<Emote> Emotes()
    {
        if (_emotes != null)
            return _emotes;

        var sheet = ExcelSheetHelper.GetSheet<Emote>();

        _emotes = sheet == null
            ? []
            : sheet.Where(emote => CommonHelper.GetEmotePlayType(emote) != EmotePlayType.DoNotPlay)
                   .Where(CommonHelper.IsEmoteDisplayable)
                   .OrderByDescending(emote => emote.RowId)
                   .ToList();

        return _emotes;
    }
}
#endif
