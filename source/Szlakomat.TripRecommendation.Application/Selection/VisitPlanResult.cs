using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Selection;

public sealed record VisitPlanResult(
    string PlanningSessionId,
    IReadOnlyCollection<VisitPlanCandidate> Selected,
    IReadOnlyCollection<VisitPlanCandidate> Rejected,
    IReadOnlyCollection<VisitPlanCandidate> Skipped,
    Money TotalPrice,
    int TotalDurationMinutes
);
