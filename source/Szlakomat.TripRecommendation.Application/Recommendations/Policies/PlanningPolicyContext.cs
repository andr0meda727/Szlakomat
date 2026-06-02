using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public sealed record PlanningPolicyContext(
    CorrectedPlanningInput Input,
    ScoringCandidate Candidate,
    BaseWorthScore BaseScore);
