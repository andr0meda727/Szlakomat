using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Sessions;

public sealed record PlanningSessionSnapshot(
    string PlanningSessionId,
    string? UserId,
    IReadOnlyCollection<string> AttractionIds,
    TravelPeriod TravelPeriod,
    string TargetCurrency,
    UserPlanningPreferences Preferences,
    PlanningConstraints Constraints
);
