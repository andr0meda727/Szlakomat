using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public interface IPlanningPolicyFactory
{
    IReadOnlyList<IPlanningPolicy> CreatePolicies(CorrectedPlanningInput input);
}
