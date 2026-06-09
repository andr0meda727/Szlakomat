using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Selection;

public sealed record VisitPlanCandidate(
    string AttractionId,
    string DisplayName,
    IReadOnlyCollection<AttractionCategory> Categories,
    VisitPlanCandidateStatus Status,
    decimal BaseScore,
    decimal FinalScore,
    Money Price,
    int EstimatedDurationMinutes,
    bool IsRequired,
    IReadOnlyCollection<string> SelectionReasons,
    IReadOnlyCollection<PlanningPolicyDecision> PolicyDecisions
);
