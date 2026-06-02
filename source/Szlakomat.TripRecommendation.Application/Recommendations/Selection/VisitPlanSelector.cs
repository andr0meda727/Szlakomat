using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Selection;

public sealed class VisitPlanSelector
{
    private const string SelectorPolicyId = "visit-plan-selector";

    public VisitPlanResult Select(
        CorrectedPlanningInput input,
        IReadOnlyList<CandidatePolicyEvaluation> evaluations)
    {
        var selected = new List<VisitPlanCandidate>();
        var rejected = new List<VisitPlanCandidate>();
        var totalPrice = 0m;
        var totalDuration = 0;

        foreach (var evaluation in evaluations.Where(evaluation => evaluation.IsRejected))
        {
            rejected.Add(ToCandidate(evaluation, input.TargetCurrency, VisitPlanCandidateStatus.Rejected));
        }

        foreach (var evaluation in SortEligible(evaluations))
        {
            if (input.Constraints.MaxAttractions is { } maxAttractions &&
                selected.Count >= maxAttractions)
            {
                rejected.Add(ToCandidate(
                    evaluation,
                    input.TargetCurrency,
                    VisitPlanCandidateStatus.Skipped,
                    SkippedDecision("SKIPPED_MAX_ATTRACTIONS", "Max attractions limit has been reached.")));
                continue;
            }

            var nextPrice = totalPrice + evaluation.Candidate.PriceInTargetCurrency;
            if (input.Constraints.MaxTotalBudget is { } maxBudget && nextPrice > maxBudget)
            {
                rejected.Add(ToCandidate(
                    evaluation,
                    input.TargetCurrency,
                    VisitPlanCandidateStatus.Skipped,
                    SkippedDecision("SKIPPED_BUDGET_LIMIT", "Candidate would exceed total budget.")));
                continue;
            }

            var nextDuration = totalDuration + evaluation.Candidate.EstimatedDurationMinutes;
            if (input.Constraints.MaxTotalDurationMinutes is { } maxDuration && nextDuration > maxDuration)
            {
                rejected.Add(ToCandidate(
                    evaluation,
                    input.TargetCurrency,
                    VisitPlanCandidateStatus.Skipped,
                    SkippedDecision("SKIPPED_DURATION_LIMIT", "Candidate would exceed total duration.")));
                continue;
            }

            selected.Add(ToCandidate(evaluation, input.TargetCurrency, VisitPlanCandidateStatus.Selected));
            totalPrice = nextPrice;
            totalDuration = nextDuration;
        }

        return new VisitPlanResult(
            input.PlanningSessionId,
            selected,
            rejected,
            totalPrice,
            totalDuration,
            input.TargetCurrency);
    }

    private static IEnumerable<CandidatePolicyEvaluation> SortEligible(
        IReadOnlyList<CandidatePolicyEvaluation> evaluations) =>
        evaluations
            .Where(evaluation => !evaluation.IsRejected)
            .OrderByDescending(evaluation => evaluation.IsRequired)
            .ThenByDescending(evaluation => evaluation.FinalScore)
            .ThenBy(evaluation => evaluation.Candidate.PriceInTargetCurrency)
            .ThenBy(evaluation => evaluation.Candidate.EstimatedDurationMinutes)
            .ThenBy(evaluation => evaluation.Candidate.AttractionId, StringComparer.Ordinal);

    private static VisitPlanCandidate ToCandidate(
        CandidatePolicyEvaluation evaluation,
        string targetCurrency,
        VisitPlanCandidateStatus status,
        PlanningPolicyDecision? extraDecision = null)
    {
        var decisions = extraDecision is null
            ? evaluation.Decisions
            : evaluation.Decisions.Concat([extraDecision]).ToArray();

        return new VisitPlanCandidate(
            evaluation.Candidate.AttractionId,
            evaluation.Candidate.DisplayName,
            evaluation.FinalScore,
            evaluation.IsRequired,
            evaluation.Candidate.EstimatedDurationMinutes,
            evaluation.Candidate.PriceInTargetCurrency,
            targetCurrency,
            evaluation.Candidate.Categories,
            status,
            decisions);
    }

    private static PlanningPolicyDecision SkippedDecision(string reasonCode, string explanation) =>
        new(
            PlanningPolicyDecisionType.Warning,
            SelectorPolicyId,
            reasonCode,
            explanation);
}
