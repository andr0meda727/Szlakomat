using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

public sealed record CandidatePolicyEvaluation(
    ScoringCandidate Candidate,
    BaseWorthScore BaseScore,
    decimal FinalScore,
    bool IsRejected,
    bool IsRequired,
    IReadOnlyList<PlanningPolicyDecision> Decisions);
