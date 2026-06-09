namespace Szlakomat.TripRecommendation.Application.Policies;

public sealed record PlanningPolicyDecision(
    PlanningPolicyDecisionType Type,
    string Reason,
    decimal ScoreDelta = 0m
)
{
    public static PlanningPolicyDecision Reject(string reason) =>
        new(PlanningPolicyDecisionType.Reject, reason);

    public static PlanningPolicyDecision Warning(string reason) =>
        new(PlanningPolicyDecisionType.Warning, reason);

    public static PlanningPolicyDecision ScoreAdjustment(decimal delta, string reason) =>
        new(PlanningPolicyDecisionType.ScoreAdjustment, reason, delta);

    public static PlanningPolicyDecision Require(string reason) =>
        new(PlanningPolicyDecisionType.Require, reason);

    public static PlanningPolicyDecision NoOpinion(string reason) =>
        new(PlanningPolicyDecisionType.NoOpinion, reason);
}
