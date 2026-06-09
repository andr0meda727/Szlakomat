using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Normalization;

public sealed record NormalizedAttractionData(
    string AttractionId,
    decimal ScoreNormalized,
    Money Price,
    TicketAvailability TicketAvailability
);
