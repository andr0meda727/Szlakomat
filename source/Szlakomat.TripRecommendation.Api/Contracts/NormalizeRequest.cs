namespace Szlakomat.TripRecommendation.Api.Contracts;

public sealed record NormalizeRequest(
    string UserId,
    IReadOnlySet<string> AttractionIds,
    DateOnly TravelDate,
    string TargetCurrency
);