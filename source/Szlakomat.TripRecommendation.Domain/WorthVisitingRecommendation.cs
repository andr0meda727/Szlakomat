namespace TripRecommendation;

public sealed record WorthVisitingRecommendation(
    string AttractionId,
    string DisplayName,
    decimal WorthScore,
    decimal ScoreNormalized,
    decimal PreferenceNormalized,
    decimal PriceScore,
    decimal AvailabilityRatio,
    decimal PriceInTargetCurrency,
    string TargetCurrency,
    int EstimatedDurationMinutes,
    bool IsTicketAvailable,
    WorthVisitingLevel Level,
    string Reason
);
