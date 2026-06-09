using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Selection;

internal sealed class VisitPlanSelector : IVisitPlanSelector
{
    public VisitPlanResult Select(
        CorrectedPlanningInput input,
        IReadOnlyCollection<CandidatePolicyEvaluation> evaluations
    )
    {
        var selected = new List<VisitPlanCandidate>();
        var rejected = evaluations
            .Where(evaluation => evaluation.IsRejected)
            .Select(evaluation => ToCandidate(
                evaluation,
                VisitPlanCandidateStatus.Rejected,
                RejectionReasons(evaluation)
            ))
            .ToList();
        var skipped = new List<VisitPlanCandidate>();

        var totalPrice = 0m;
        var totalDuration = 0;

        foreach (var evaluation in Ranked(evaluations.Where(evaluation => !evaluation.IsRejected)))
        {
            var skipReasons = SkipReasons(input, evaluation, selected.Count, totalPrice, totalDuration);

            if (skipReasons.Count > 0)
            {
                skipped.Add(ToCandidate(evaluation, VisitPlanCandidateStatus.Skipped, skipReasons));
                continue;
            }

            selected.Add(ToCandidate(
                evaluation,
                VisitPlanCandidateStatus.Selected,
                ["Selected by rule engine ranking."]
            ));

            totalPrice += evaluation.Candidate.Price.Amount;
            totalDuration += evaluation.Candidate.EstimatedDurationMinutes;
        }

        return new VisitPlanResult(
            input.PlanningSessionId,
            selected,
            rejected,
            skipped,
            new Money(totalPrice, input.TargetCurrency),
            totalDuration
        );
    }

    private static IEnumerable<CandidatePolicyEvaluation> Ranked(IEnumerable<CandidatePolicyEvaluation> evaluations)
    {
        return evaluations
            .OrderByDescending(evaluation => evaluation.IsRequired)
            .ThenByDescending(evaluation => evaluation.FinalScore)
            .ThenBy(evaluation => evaluation.Candidate.Price.Amount)
            .ThenBy(evaluation => evaluation.Candidate.EstimatedDurationMinutes)
            .ThenBy(evaluation => evaluation.Candidate.AttractionId, StringComparer.Ordinal);
    }

    private static List<string> SkipReasons(
        CorrectedPlanningInput input,
        CandidatePolicyEvaluation evaluation,
        int selectedCount,
        decimal currentPrice,
        int currentDuration
    )
    {
        var reasons = new List<string>();

        if (input.Constraints.MaxAttractions.HasValue
            && selectedCount >= input.Constraints.MaxAttractions.Value)
        {
            reasons.Add("Maximum number of attractions was reached.");
        }

        if (input.Constraints.MaxTotalBudget.HasValue
            && currentPrice + evaluation.Candidate.Price.Amount > input.Constraints.MaxTotalBudget.Value)
        {
            reasons.Add("Candidate does not fit the remaining trip budget.");
        }

        if (input.Constraints.MaxTotalDurationMinutes.HasValue
            && currentDuration + evaluation.Candidate.EstimatedDurationMinutes
                > input.Constraints.MaxTotalDurationMinutes.Value)
        {
            reasons.Add("Candidate does not fit the remaining trip duration.");
        }

        return reasons;
    }

    private static IReadOnlyCollection<string> RejectionReasons(CandidatePolicyEvaluation evaluation)
    {
        var reasons = evaluation.Decisions
            .Where(decision => decision.Type == PlanningPolicyDecisionType.Reject)
            .Select(decision => decision.Reason)
            .ToArray();

        return reasons.Length > 0 ? reasons : ["Rejected by planning policy."];
    }

    private static VisitPlanCandidate ToCandidate(
        CandidatePolicyEvaluation evaluation,
        VisitPlanCandidateStatus status,
        IReadOnlyCollection<string> selectionReasons
    )
    {
        return new VisitPlanCandidate(
            evaluation.Candidate.AttractionId,
            evaluation.Candidate.DisplayName,
            evaluation.Candidate.Categories,
            status,
            evaluation.BaseScore,
            evaluation.FinalScore,
            evaluation.Candidate.Price,
            evaluation.Candidate.EstimatedDurationMinutes,
            evaluation.IsRequired,
            selectionReasons,
            evaluation.Decisions
        );
    }
}
