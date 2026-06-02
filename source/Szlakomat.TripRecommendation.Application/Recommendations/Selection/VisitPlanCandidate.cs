using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Selection;

public sealed record VisitPlanCandidate(
    string AttractionId,
    string DisplayName,
    decimal PriorityScore,
    bool IsRequired,
    int EstimatedDurationMinutes,
    decimal PriceInTargetCurrency,
    string TargetCurrency,
    IReadOnlySet<AttractionCategory> Categories,
    VisitPlanCandidateStatus Status,
    IReadOnlyList<PlanningPolicyDecision> Decisions);
