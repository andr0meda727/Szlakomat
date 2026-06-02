namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public enum PlanningPolicyDecisionType
{
    NoOpinion = 0,
    Reject = 1,
    Require = 2,
    ScoreAdjustment = 3,
    Warning = 4
}
