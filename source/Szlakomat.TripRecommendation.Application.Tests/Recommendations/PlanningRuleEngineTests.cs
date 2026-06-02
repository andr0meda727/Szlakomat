using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class PlanningRuleEngineTests
{
    [Fact]
    public async Task EvaluateAsync_PreservesMultiplePolicyDecisionsAndAppliesScoreDelta()
    {
        var candidate = PlanningTestData.Candidate("a");
        var input = PlanningTestData.Input(candidates: [candidate]);
        var baseScores = Scores(new BaseWorthScore("a", 0.50m, 0.50m, 0.50m, 0.50m, 0.50m));
        var policy = new StubPolicy("data-quality", 10, [
            Decision(PlanningPolicyDecisionType.Warning, "LOW_CONFIDENCE", "Low confidence."),
            Decision(PlanningPolicyDecisionType.ScoreAdjustment, "LOW_CONFIDENCE_PENALTY", "Penalty.", -0.10m)
        ]);

        var result = await new PlanningRuleEngine()
            .EvaluateAsync(input, baseScores, [policy], CancellationToken.None);

        var evaluation = Assert.Single(result);
        Assert.False(evaluation.IsRejected);
        Assert.False(evaluation.IsRequired);
        Assert.Equal(0.40m, evaluation.FinalScore);
        Assert.Equal(["LOW_CONFIDENCE", "LOW_CONFIDENCE_PENALTY"], evaluation.Decisions.Select(x => x.ReasonCode));
    }

    [Fact]
    public async Task EvaluateAsync_HardRejectWinsOverRequire()
    {
        var candidate = PlanningTestData.Candidate("a");
        var input = PlanningTestData.Input(candidates: [candidate]);
        var baseScores = Scores(new BaseWorthScore("a", 0.90m, 0.90m, 0.90m, 0.90m, 0.90m));
        var policies = new IPlanningPolicy[]
        {
            new StubPolicy("must-see", 10, [Decision(PlanningPolicyDecisionType.Require, "MUST_SEE", "Must see.")]),
            new StubPolicy("availability", 20, [Decision(PlanningPolicyDecisionType.Reject, "NO_TICKETS", "No tickets.", isHard: true)])
        };

        var result = await new PlanningRuleEngine()
            .EvaluateAsync(input, baseScores, policies, CancellationToken.None);

        var evaluation = Assert.Single(result);
        Assert.True(evaluation.IsRejected);
        Assert.True(evaluation.IsRequired);
        Assert.Equal(0.90m, evaluation.FinalScore);
    }

    private static IReadOnlyDictionary<string, BaseWorthScore> Scores(params BaseWorthScore[] scores) =>
        scores.ToDictionary(x => x.AttractionId);

    private static PlanningPolicyDecision Decision(
        PlanningPolicyDecisionType type,
        string code,
        string explanation,
        decimal delta = 0m,
        bool isHard = false) =>
        new(type, "policy", code, explanation, delta, isHard);

    private sealed class StubPolicy(
        string id,
        int order,
        IReadOnlyList<PlanningPolicyDecision> decisions)
        : IPlanningPolicy
    {
        public string Id { get; } = id;
        public int Order { get; } = order;

        public ValueTask<IReadOnlyList<PlanningPolicyDecision>> EvaluateAsync(
            PlanningPolicyContext context,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult(decisions);
    }
}
