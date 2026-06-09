using Szlakomat.TripRecommendation.Application.Sessions;

namespace Szlakomat.TripRecommendation.Application.Normalization;

public interface IPlanningInputNormalizer
{
    Task<IReadOnlyDictionary<string, NormalizedAttractionData>> NormalizeAsync(
        PlanningSessionSnapshot session,
        CancellationToken cancellationToken
    );
}
