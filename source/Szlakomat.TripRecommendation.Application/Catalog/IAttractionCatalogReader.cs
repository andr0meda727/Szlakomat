using Szlakomat.TripRecommendation.Application.Sessions;

namespace Szlakomat.TripRecommendation.Application.Catalog;

public interface IAttractionCatalogReader
{
    Task<IReadOnlyDictionary<string, AttractionCatalogItem>> GetAsync(
        PlanningSessionSnapshot session,
        CancellationToken cancellationToken
    );
}
