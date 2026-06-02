using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;
using Szlakomat.TripRecommendation.Application.Recommendations.Selection;
using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class VisitPlanSelectorTests
{
    [Fact]
    public void Select_RemovesRejectedAndGreedilyAppliesBudgetDurationAndCountLimits()
    {
        var input = PlanningTestData.Input(
            new PlanningConstraints(100m, 2, 180, false),
            PlanningTestData.Candidate("a", price: 70m, duration: 90),
            PlanningTestData.Candidate("b", price: 60m, duration: 90),
            PlanningTestData.Candidate("c", price: 30m, duration: 90),
            PlanningTestData.Candidate("d", price: 10m, duration: 30));

        var evaluations = new[]
        {
            Evaluation(input.Candidates[0], 0.95m),
            Evaluation(input.Candidates[1], 0.90m),
            Evaluation(input.Candidates[2], 0.80m),
            Evaluation(input.Candidates[3], 1.00m, rejected: true)
        };

        var result = new VisitPlanSelector().Select(input, evaluations);

        Assert.Equal(["a", "c"], result.Selected.Select(x => x.AttractionId));
        Assert.Equal(100m, result.TotalPrice);
        Assert.Equal(180, result.TotalDurationMinutes);
        Assert.Contains(result.Rejected, x => x.AttractionId == "d" && x.Status == VisitPlanCandidateStatus.Rejected);
        Assert.Contains(result.Rejected, x => x.AttractionId == "b" && x.Status == VisitPlanCandidateStatus.Skipped);
    }

    [Fact]
    public void Select_WhenNoLimits_ReturnsAllNonRejectedCandidatesSortedByPriority()
    {
        var input = PlanningTestData.Input(
            candidates:
            [
                PlanningTestData.Candidate("b", price: 20m),
                PlanningTestData.Candidate("a", price: 10m),
                PlanningTestData.Candidate("c", price: 5m)
            ]);

        var evaluations = new[]
        {
            Evaluation(input.Candidates[0], 0.80m),
            Evaluation(input.Candidates[1], 0.80m),
            Evaluation(input.Candidates[2], 0.30m, rejected: true)
        };

        var result = new VisitPlanSelector().Select(input, evaluations);

        Assert.Equal(["a", "b"], result.Selected.Select(x => x.AttractionId));
    }

    private static CandidatePolicyEvaluation Evaluation(
        ScoringCandidate candidate,
        decimal finalScore,
        bool rejected = false,
        bool required = false,
        IReadOnlyList<PlanningPolicyDecision>? decisions = null) =>
        new(
            candidate,
            new BaseWorthScore(candidate.AttractionId, finalScore, finalScore, finalScore, finalScore, finalScore),
            finalScore,
            rejected,
            required,
            decisions ?? Array.Empty<PlanningPolicyDecision>());
}
