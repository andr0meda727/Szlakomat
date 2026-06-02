using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public sealed class DefaultPlanningPolicyFactory(IEnumerable<IPlanningPolicy> policies)
    : IPlanningPolicyFactory
{
    public IReadOnlyList<IPlanningPolicy> CreatePolicies(CorrectedPlanningInput input) =>
        policies
            .OrderBy(policy => policy.Order)
            .ThenBy(policy => policy.Id, StringComparer.Ordinal)
            .ToArray();
}
