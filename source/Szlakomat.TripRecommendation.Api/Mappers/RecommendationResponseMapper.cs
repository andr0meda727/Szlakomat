using Szlakomat.TripRecommendation.Api.Contracts.Recommendations;
using Szlakomat.TripRecommendation.Application.Selection;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Api.Mappers;

internal static class RecommendationResponseMapper
{
    public static PlanRecommendationsResponse ToResponse(this VisitPlanResult result)
    {
        return new PlanRecommendationsResponse(
            result.PlanningSessionId,
            result.Selected.Select(ToResponse).ToArray(),
            result.TotalPrice.ToResponse(),
            result.TotalDurationMinutes
        );
    }

    private static RecommendedAttractionResponse ToResponse(VisitPlanCandidate candidate)
    {
        return new RecommendedAttractionResponse(
            candidate.AttractionId,
            candidate.DisplayName,
            candidate.Categories.Select(category => category.ToString()).ToArray(),
            candidate.Price.ToResponse(),
            candidate.EstimatedDurationMinutes
        );
    }

    private static MoneyResponse ToResponse(this Money money)
    {
        return new MoneyResponse(money.Amount, money.CurrencyCode);
    }
}
