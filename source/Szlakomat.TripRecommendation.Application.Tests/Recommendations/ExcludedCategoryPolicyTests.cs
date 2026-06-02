using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class ExcludedCategoryPolicyTests
{
    [Fact]
    public void AppliesTo_WhenExcludedCategoriesEmpty_ReturnsFalse()
    {
        var input = PlanningTestData.Input(PlanningConstraints.None);
        Assert.False(new ExcludedCategoryPolicy().AppliesTo(input));
    }

    [Fact]
    public void AppliesTo_WhenExcludedCategoriesNonEmpty_ReturnsTrue()
    {
        var input = InputWithExcluded(AttractionCategory.Museum);
        Assert.True(new ExcludedCategoryPolicy().AppliesTo(input));
    }

    [Fact]
    public async Task EvaluateAsync_WhenCandidateCategoryIsExcluded_ReturnsHardReject()
    {
        var candidate = PlanningTestData.Candidate("museum-1");
        var input = InputWithExcluded(AttractionCategory.Museum, candidate);

        var decisions = await new ExcludedCategoryPolicy().EvaluateAsync(
            Context(input, candidate),
            CancellationToken.None);

        var decision = Assert.Single(decisions);
        Assert.Equal(PlanningPolicyDecisionType.Reject, decision.Type);
        Assert.True(decision.IsHardConstraint);
        Assert.Equal("EXCLUDED_CATEGORY", decision.ReasonCode);
    }

    [Fact]
    public async Task EvaluateAsync_WhenCandidateCategoryNotExcluded_ReturnsNoOpinion()
    {
        var candidate = CandidateWithCategory("park-1", AttractionCategory.Outdoor);
        var input = InputWithExcluded(AttractionCategory.Museum, candidate);

        var decisions = await new ExcludedCategoryPolicy().EvaluateAsync(
            Context(input, candidate),
            CancellationToken.None);

        var decision = Assert.Single(decisions);
        Assert.Equal(PlanningPolicyDecisionType.NoOpinion, decision.Type);
        Assert.Equal("CATEGORY_NOT_EXCLUDED", decision.ReasonCode);
    }

    private static ScoringCandidate CandidateWithCategory(string id, AttractionCategory category) =>
        new(
            AttractionId: id,
            DisplayName: id,
            Categories: new HashSet<AttractionCategory> { category },
            ScoreNormalized: 0.7m,
            PriceInTargetCurrency: 50m,
            PreferenceNormalized: 0.7m,
            AvailabilityRatio: 0.7m,
            IsTicketAvailable: true,
            EstimatedDurationMinutes: 60);

    private static CorrectedPlanningInput InputWithExcluded(
        AttractionCategory excluded,
        params ScoringCandidate[] candidates) =>
        new(
            PlanningSessionId: "session-1",
            UserId: "user-1",
            TravelDate: new DateOnly(2026, 6, 10),
            TargetCurrency: "PLN",
            Candidates: candidates,
            Preferences: new UserPlanningPreferences(
                PreferredCategories: new HashSet<AttractionCategory>(),
                ExcludedCategories: new HashSet<AttractionCategory> { excluded },
                Weights: ScoringWeights.Default),
            Constraints: PlanningConstraints.None);

    private static PlanningPolicyContext Context(
        CorrectedPlanningInput input,
        ScoringCandidate candidate) =>
        new(input, candidate, new BaseWorthScore(candidate.AttractionId, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m));
}
