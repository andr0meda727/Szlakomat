namespace Szlakomat.TripRecommendation.Api.Contracts;

public sealed record CreatePlanningSnapshotRequest(
    string UserId,
    List<string> AttractionIds,
    DateOnly TravelDate,
    string TargetCurrency,
    ScoringWeightsDto? Weights                 = null,
    List<string>? PreferredCategories          = null,
    List<string>? ExcludedCategories           = null,
    decimal? MaxBudget                         = null,
    int? MaxAttractions                        = null,
    int? MaxTotalDurationMinutes               = null,
    bool RequireTicketAvailability             = false
);

public sealed record ScoringWeightsDto(
    decimal ScoreWeight,
    decimal PreferenceWeight,
    decimal PriceWeight,
    decimal AvailabilityWeight
);
