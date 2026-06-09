namespace Szlakomat.TripRecommendation.Domain.Planning;

public sealed record UserPlanningPreferences(
    IReadOnlyCollection<AttractionCategory> PreferredCategories,
    IReadOnlyCollection<AttractionCategory> ExcludedCategories,
    IReadOnlyCollection<string> MustSeeAttractionIds
);
