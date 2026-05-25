namespace TripRecommendation;

public sealed record NormalizedAttractionData(
    string AttractionId,
    decimal ScoreNormalized,
    decimal PriceInTargetCurrency,
    string TargetCurrency,
    decimal PreferenceNormalized,
    decimal AvailabilityRatio,
    bool IsTicketAvailable
);