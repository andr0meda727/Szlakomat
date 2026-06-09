using Szlakomat.TripRecommendation.Application.Sessions;

namespace Szlakomat.TripRecommendation.Application.External;

public interface IExternalScoringProvider
{
    Task<IReadOnlyDictionary<string, ExternalAttractionScore>> GetAsync(
        PlanningSessionSnapshot session,
        CancellationToken cancellationToken
    );
}
