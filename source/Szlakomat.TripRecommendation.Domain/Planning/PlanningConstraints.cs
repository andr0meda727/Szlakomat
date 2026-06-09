namespace Szlakomat.TripRecommendation.Domain.Planning;

public sealed record PlanningConstraints(
    decimal? MaxTotalBudget,
    int? MaxAttractions,
    int? MaxTotalDurationMinutes
);
