using Szlakomat.TripRecommendation.Application.Planning;
using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

public sealed class WorthVisitingBaseScorer : IBaseWorthScorer
{
    private const decimal PreferredCategoryBoost = 0.10m;

    public IReadOnlyDictionary<string, BaseWorthScore> Score(CorrectedPlanningInput input)
    {
        var candidates = input.Candidates
            .Select(candidate => ToRankingCandidate(candidate, input))
            .ToArray();

        var settings = new WorthVisitingRankingSettings(
            ScoreWeight: input.Preferences.Weights.ScoreWeight,
            PreferenceWeight: input.Preferences.Weights.PreferenceWeight,
            PriceWeight: input.Preferences.Weights.PriceWeight,
            AvailabilityWeight: input.Preferences.Weights.AvailabilityWeight,
            MaxTotalBudget: null,
            MaxAttractions: null,
            MaxTotalDurationMinutes: null,
            RequireTicketAvailability: false);

        return WorthVisitingRanker.Rank(candidates, settings)
            .ToDictionary(
                recommendation => recommendation.AttractionId,
                recommendation => new BaseWorthScore(
                    recommendation.AttractionId,
                    recommendation.WorthScore,
                    recommendation.ScoreNormalized,
                    recommendation.PreferenceNormalized,
                    recommendation.PriceScore,
                    recommendation.AvailabilityRatio),
                StringComparer.Ordinal);
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
