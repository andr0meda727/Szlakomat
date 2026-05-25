using MediatR;
using Szlakomat.TripRecommendation.Application.Planning;
using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application;

internal sealed class DetermineWorthVisitingHandler
    : IRequestHandler<DetermineWorthVisiting, IReadOnlyList<WorthVisitingRecommendation>>
{
    private const decimal PreferredCategoryBoost = 0.10m;

    public Task<IReadOnlyList<WorthVisitingRecommendation>> Handle(
        DetermineWorthVisiting request,
        CancellationToken cancellationToken)
    {
        var input = request.PlanningInput;
        var candidates = input.Candidates
            .Where(x => !HasAnyCategory(x, input.Preferences.ExcludedCategories))
            .Select(x => ToRankingCandidate(x, input))
            .ToArray();

        var settings = new WorthVisitingRankingSettings(
            ScoreWeight: input.Preferences.Weights.ScoreWeight,
            PreferenceWeight: input.Preferences.Weights.PreferenceWeight,
            PriceWeight: input.Preferences.Weights.PriceWeight,
            AvailabilityWeight: input.Preferences.Weights.AvailabilityWeight,
            MaxTotalBudget: input.Constraints.MaxTotalBudget,
            MaxAttractions: input.Constraints.MaxAttractions,
            MaxTotalDurationMinutes: input.Constraints.MaxTotalDurationMinutes,
            RequireTicketAvailability: input.Constraints.RequireTicketAvailability);

        return Task.FromResult(WorthVisitingRanker.Rank(candidates, settings));
    }

    private static WorthVisitingCandidate ToRankingCandidate(
        ScoringCandidate candidate,
        CorrectedPlanningInput input)
    {
        var preference = HasAnyCategory(candidate, input.Preferences.PreferredCategories)
            ? NormalizationMath.Clamp01(candidate.PreferenceNormalized + PreferredCategoryBoost)
            : candidate.PreferenceNormalized;

        return new WorthVisitingCandidate(
            AttractionId: candidate.AttractionId,
            DisplayName: candidate.DisplayName,
            ScoreNormalized: candidate.ScoreNormalized,
            PriceInTargetCurrency: candidate.PriceInTargetCurrency,
            TargetCurrency: input.TargetCurrency,
            PreferenceNormalized: preference,
            AvailabilityRatio: candidate.AvailabilityRatio,
            IsTicketAvailable: candidate.IsTicketAvailable,
            EstimatedDurationMinutes: candidate.EstimatedDurationMinutes);
    }

    private static bool HasAnyCategory(
        ScoringCandidate candidate,
        IReadOnlySet<AttractionCategory> categories) =>
        categories.Count != 0 && candidate.Categories.Any(categories.Contains);
}
