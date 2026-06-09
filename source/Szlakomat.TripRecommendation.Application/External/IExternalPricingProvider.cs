using Szlakomat.TripRecommendation.Application.Sessions;

namespace Szlakomat.TripRecommendation.Application.External;

public interface IExternalPricingProvider
{
    Task<IReadOnlyDictionary<string, ExternalAttractionPrice>> GetAsync(
        PlanningSessionSnapshot session,
        CancellationToken cancellationToken
    );
}
