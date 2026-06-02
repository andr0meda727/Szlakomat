using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Input;

public interface IPlanningInputProvider
{
    Task<CorrectedPlanningInput> GetAsync(
        string planningSessionId,
        CancellationToken cancellationToken);
}
