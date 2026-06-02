using Szlakomat.TripRecommendation.Api.Contracts.Recommendations;
using Szlakomat.TripRecommendation.Application.Recommendations.Selection;

namespace Szlakomat.TripRecommendation.Api.Mappers;

public static class RecommendationMapper
{
    public static PlanRecommendationsResponse ToResponse(VisitPlanResult result) =>
        new(
            result.PlanningSessionId,
            result.Selected.Select(ToResponse).ToArray(),
            result.Rejected.Select(ToResponse).ToArray(),
            result.TotalPrice,
            result.TotalDurationMinutes,
            result.TargetCurrency);

    private static VisitPlanAttractionResponse ToResponse(VisitPlanCandidate candidate) =>
        new(
            candidate.AttractionId,
            candidate.DisplayName,
            candidate.PriorityScore,
            candidate.IsRequired,
            candidate.EstimatedDurationMinutes,
            candidate.PriceInTargetCurrency,
            candidate.TargetCurrency,
            candidate.Categories.Select(category => category.ToString()).ToArray(),
            candidate.Status.ToString(),
            candidate.Decisions.Select(decision => new VisitPlanDecisionResponse(
                decision.Type.ToString(),
                decision.PolicyId,
                decision.ReasonCode,
                decision.Explanation,
                decision.ScoreDelta,
                decision.IsHardConstraint)).ToArray());
}
