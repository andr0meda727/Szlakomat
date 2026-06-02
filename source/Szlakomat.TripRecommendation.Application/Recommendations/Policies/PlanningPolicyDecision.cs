namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public sealed record PlanningPolicyDecision(
    PlanningPolicyDecisionType Type,
    string PolicyId,
    string ReasonCode,
    string Explanation,
    decimal ScoreDelta = 0m,
    bool IsHardConstraint = false);
