using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Catalog;

public sealed record AttractionCatalogItem(
    string AttractionId,
    string DisplayName,
    IReadOnlyCollection<AttractionCategory> Categories,
    int EstimatedDurationMinutes
);
