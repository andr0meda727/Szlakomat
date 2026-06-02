namespace Szlakomat.TripRecommendation.Api.Contracts.Recommendations;

public sealed record PlanRecommendationsResponse(
    string PlanningSessionId,
    IReadOnlyList<VisitPlanAttractionResponse> Selected,
    IReadOnlyList<VisitPlanAttractionResponse> Rejected,
    decimal TotalPrice,
    int TotalDurationMinutes,
    string TargetCurrency);

public sealed record VisitPlanAttractionResponse(
    string AttractionId,
    string DisplayName,
    decimal PriorityScore,
    bool IsRequired,
    int EstimatedDurationMinutes,
    decimal PriceInTargetCurrency,
    string TargetCurrency,
    IReadOnlyList<string> Categories,
    string Status,
    IReadOnlyList<VisitPlanDecisionResponse> Decisions);

public sealed record VisitPlanDecisionResponse(
    string Type,
    string PolicyId,
    string ReasonCode,
    string Explanation,
    decimal ScoreDelta,
    bool IsHardConstraint);
