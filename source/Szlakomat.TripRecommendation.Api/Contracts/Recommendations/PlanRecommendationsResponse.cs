namespace Szlakomat.TripRecommendation.Api.Contracts.Recommendations;

public sealed record PlanRecommendationsResponse(
    string PlanningSessionId,
    IReadOnlyCollection<RecommendedAttractionResponse> Attractions,
    MoneyResponse TotalPrice,
    int TotalDurationMinutes
);

public sealed record RecommendedAttractionResponse(
    string AttractionId,
    string DisplayName,
    IReadOnlyCollection<string> Categories,
    MoneyResponse Price,
    int EstimatedDurationMinutes
);

public sealed record MoneyResponse(decimal Amount, string CurrencyCode);
