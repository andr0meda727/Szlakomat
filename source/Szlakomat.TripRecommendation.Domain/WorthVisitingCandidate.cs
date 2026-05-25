namespace TripRecommendation;

public sealed record WorthVisitingCandidate(
    string AttractionId,
    string DisplayName,
    decimal ScoreNormalized,
    decimal PriceInTargetCurrency,
    string TargetCurrency,
    decimal PreferenceNormalized,
    decimal AvailabilityRatio,
    bool IsTicketAvailable,
    int EstimatedDurationMinutes
);
