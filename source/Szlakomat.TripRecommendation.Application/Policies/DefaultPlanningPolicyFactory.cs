using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

internal sealed class DefaultPlanningPolicyFactory(IEnumerable<IPlanningPolicy> policies) : IPlanningPolicyFactory
{
    public IReadOnlyCollection<IPlanningPolicy> CreateFor(CorrectedPlanningInput input) =>
        policies
            .Where(p => p.IsEnabledFor(input))
            .OrderBy(p => p.Order)
            .ToList();
}
