using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Planning;

public interface ICorrectedPlanningInputFactory
{
    Task<CorrectedPlanningInput> CreateAsync(string planningSessionId, CancellationToken cancellationToken);
}
