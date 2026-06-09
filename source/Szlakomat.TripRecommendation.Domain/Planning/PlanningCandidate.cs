namespace Szlakomat.TripRecommendation.Domain.Planning;

public sealed record PlanningCandidate(
    string AttractionId,
    string DisplayName,
    IReadOnlyCollection<AttractionCategory> Categories,
    decimal ScoreNormalized,
    Money Price,
    int EstimatedDurationMinutes,
    TicketAvailability TicketAvailability
);
