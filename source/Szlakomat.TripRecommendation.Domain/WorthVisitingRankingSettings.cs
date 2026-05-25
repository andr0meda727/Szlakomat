namespace TripRecommendation;

public sealed record WorthVisitingRankingSettings(
    decimal ScoreWeight,
    decimal PreferenceWeight,
    decimal PriceWeight,
    decimal AvailabilityWeight,
    decimal? MaxTotalBudget,
    int? MaxAttractions,
    int? MaxTotalDurationMinutes,
    bool RequireTicketAvailability
)
{
    public decimal WeightSum => ScoreWeight + PreferenceWeight + PriceWeight + AvailabilityWeight;
}
