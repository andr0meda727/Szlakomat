using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class DefaultPlanningPolicyFactoryTests
{
    [Fact]
    public void CreatePolicies_FiltersOutPoliciesNotApplicableToInput()
    {
        var always = new StubPolicy("always", 10, appliesTo: true);
        var never = new StubPolicy("never", 20, appliesTo: false);
        var factory = new DefaultPlanningPolicyFactory([always, never]);
        var input = PlanningTestData.Input();

        var result = factory.CreatePolicies(input);

        Assert.Equal(["always"], result.Select(p => p.Id));
    }

    [Fact]
    public void CreatePolicies_OrdersByOrderThenId()
    {
        var b = new StubPolicy("b", 10, appliesTo: true);
        var a = new StubPolicy("a", 10, appliesTo: true);
        var c = new StubPolicy("c", 5, appliesTo: true);
        var factory = new DefaultPlanningPolicyFactory([b, a, c]);
        var input = PlanningTestData.Input();

        var result = factory.CreatePolicies(input);

        Assert.Equal(["c", "a", "b"], result.Select(p => p.Id));
    }

    private sealed class StubPolicy(string id, int order, bool appliesTo) : IPlanningPolicy
    {
        public string Id { get; } = id;
        public int Order { get; } = order;

        public bool AppliesTo(CorrectedPlanningInput input) => appliesTo;

        public ValueTask<IReadOnlyList<PlanningPolicyDecision>> EvaluateAsync(
            PlanningPolicyContext context,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([]);
    }
}
