using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

internal static class PlanningTestData
{
    internal static CorrectedPlanningInput Input(
        PlanningConstraints? constraints = null,
        params ScoringCandidate[] candidates) =>
        new(
            PlanningSessionId: "session-1",
            UserId: "user-1",
            TravelDate: new DateOnly(2026, 6, 10),
            TargetCurrency: "PLN",
            Candidates: candidates,
            Preferences: new UserPlanningPreferences(
                PreferredCategories: new HashSet<AttractionCategory>(),
                ExcludedCategories: new HashSet<AttractionCategory>(),
                Weights: ScoringWeights.Default),
            Constraints: constraints ?? PlanningConstraints.None);

    internal static ScoringCandidate Candidate(
        string id,
        decimal score = 0.7m,
        decimal preference = 0.7m,
        decimal price = 50m,
        decimal availability = 0.7m,
        bool tickets = true,
        int duration = 60) =>
        new(
            AttractionId: id,
            DisplayName: id,
            Categories: new HashSet<AttractionCategory> { AttractionCategory.Museum },
            ScoreNormalized: score,
            PriceInTargetCurrency: price,
            PreferenceNormalized: preference,
            AvailabilityRatio: availability,
            IsTicketAvailable: tickets,
            EstimatedDurationMinutes: duration);
}
