namespace Szlakomat.TripRecommendation.Application.Policies;

public enum PlanningPolicyDecisionType
{
    Reject,
    Warning,
    ScoreAdjustment,
    Require,
    NoOpinion
}
