namespace Szlakomat.TripRecommendation.Application.Recommendations.Selection;

public sealed record VisitPlanResult(
    string PlanningSessionId,
    IReadOnlyList<VisitPlanCandidate> Selected,
    IReadOnlyList<VisitPlanCandidate> Rejected,
    decimal TotalPrice,
    int TotalDurationMinutes,
    string TargetCurrency);
