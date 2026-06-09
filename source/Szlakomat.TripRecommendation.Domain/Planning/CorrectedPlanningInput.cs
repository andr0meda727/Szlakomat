namespace Szlakomat.TripRecommendation.Domain.Planning;

public sealed record CorrectedPlanningInput(
    string PlanningSessionId,
    string? UserId,
    TravelPeriod TravelPeriod,
    string TargetCurrency,
    IReadOnlyCollection<PlanningCandidate> Candidates,
    UserPlanningPreferences Preferences,
    PlanningConstraints Constraints
);
