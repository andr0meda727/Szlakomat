namespace Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

public sealed record BaseWorthScore(
    string AttractionId,
    decimal WorthScore,
    decimal ExternalScoreComponent,
    decimal PreferenceComponent,
    decimal PriceComponent,
    decimal AvailabilityComponent);
