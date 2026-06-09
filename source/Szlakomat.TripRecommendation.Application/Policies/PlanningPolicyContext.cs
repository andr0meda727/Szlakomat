using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

public sealed record PlanningPolicyContext(
    CorrectedPlanningInput Input,
    PlanningCandidate Candidate,
    decimal BaseScore
);
