using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Application.Recommendations.Input;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;
using Szlakomat.TripRecommendation.Application.Recommendations.Selection;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class PlanVisitRecommendationsHandlerTests
{
    [Fact]
    public async Task Handle_LoadsInputByPlanningSessionAndReturnsSelectedAttractions()
    {
        var input = PlanningTestData.Input(
            constraints: new PlanningConstraints(null, 1, null, false),
            PlanningTestData.Candidate("a", score: 0.90m, preference: 0.90m, price: 10m),
            PlanningTestData.Candidate("b", score: 0.10m, preference: 0.10m, price: 10m));

        var handler = new PlanVisitRecommendationsHandler(
            new FakePlanningInputProvider(input),
            new WorthVisitingBaseScorer(),
            new FakePolicyFactory([]),
            new PlanningRuleEngine(),
            new VisitPlanSelector());

        var result = await handler.Handle(new PlanVisitRecommendations("session-1"), CancellationToken.None);

        Assert.Equal("session-1", result.PlanningSessionId);
        Assert.Equal(["a"], result.Selected.Select(x => x.AttractionId));
    }

    private sealed class FakePlanningInputProvider(CorrectedPlanningInput input) : IPlanningInputProvider
    {
        public Task<CorrectedPlanningInput> GetAsync(
            string planningSessionId,
            CancellationToken cancellationToken) =>
            Task.FromResult(input);
    }

    private sealed class FakePolicyFactory(IReadOnlyList<IPlanningPolicy> policies) : IPlanningPolicyFactory
    {
        public IReadOnlyList<IPlanningPolicy> CreatePolicies(CorrectedPlanningInput input) => policies;
    }
}
