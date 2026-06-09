using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

public interface IPlanningPolicyFactory
{
    IReadOnlyCollection<IPlanningPolicy> CreateFor(CorrectedPlanningInput input);
}
