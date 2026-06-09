using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

public sealed record CandidatePolicyEvaluation(
    PlanningCandidate Candidate,
    decimal BaseScore,
    decimal FinalScore,
    bool IsRejected,
    bool IsRequired,
    IReadOnlyCollection<PlanningPolicyDecision> Decisions
);
