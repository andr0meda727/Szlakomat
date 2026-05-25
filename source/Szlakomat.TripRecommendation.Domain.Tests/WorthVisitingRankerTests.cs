using TripRecommendation;
using Xunit;

namespace Szlakomat.TripRecommendation.Domain.Tests;

public class WorthVisitingRankerTests
{
    [Fact]
    public void Rank_PrefersCheaperAttractionWhenOtherSignalsAreSimilar()
    {
        var result = WorthVisitingRanker.Rank([
            Candidate("expensive", price: 100m, score: 0.8m, preference: 0.8m, availability: 0.8m),
            Candidate("cheap", price: 20m, score: 0.78m, preference: 0.78m, availability: 0.78m)
        ], DefaultSettings());

        Assert.Equal("cheap", result[0].AttractionId);
        Assert.True(result[0].WorthScore > result[1].WorthScore);
    }

    [Fact]
    public void Rank_CanExcludeAttractionsWithoutTickets()
    {
        var settings = DefaultSettings() with { RequireTicketAvailability = true };

        var result = WorthVisitingRanker.Rank([
            Candidate("available", tickets: true),
            Candidate("sold-out", score: 1m, preference: 1m, availability: 0m, tickets: false)
        ], settings);

        Assert.Single(result);
        Assert.Equal("available", result[0].AttractionId);
    }

    [Fact]
    public void Rank_AppliesBudgetDurationAndCountLimits()
    {
        var settings = DefaultSettings() with
        {
            MaxTotalBudget = 70m,
            MaxTotalDurationMinutes = 120,
            MaxAttractions = 2
        };

        var result = WorthVisitingRanker.Rank([
            Candidate("a", price: 40m, duration: 60),
            Candidate("b", price: 40m, duration: 60),
            Candidate("c", price: 20m, duration: 60)
        ], settings);

        Assert.Equal(2, result.Count);
        Assert.True(result.Sum(x => x.PriceInTargetCurrency) <= 70m);
        Assert.True(result.Sum(x => x.EstimatedDurationMinutes) <= 120);
    }

    [Fact]
    public void Rank_ReturnsDeterministicOrderForEqualScores()
    {
        var result = WorthVisitingRanker.Rank([
            Candidate("b", price: 50m),
            Candidate("a", price: 50m)
        ], DefaultSettings());

        Assert.Equal(["a", "b"], result.Select(x => x.AttractionId));
    }

    private static WorthVisitingCandidate Candidate(
        string id,
        decimal price = 50m,
        decimal score = 0.7m,
        decimal preference = 0.7m,
        decimal availability = 0.7m,
        bool tickets = true,
        int duration = 60) =>
        new(
            AttractionId: id,
            DisplayName: id,
            ScoreNormalized: score,
            PriceInTargetCurrency: price,
            TargetCurrency: "PLN",
            PreferenceNormalized: preference,
            AvailabilityRatio: availability,
            IsTicketAvailable: tickets,
            EstimatedDurationMinutes: duration);

    private static WorthVisitingRankingSettings DefaultSettings() =>
        new(
            ScoreWeight: 0.35m,
            PreferenceWeight: 0.35m,
            PriceWeight: 0.20m,
            AvailabilityWeight: 0.10m,
            MaxTotalBudget: null,
            MaxAttractions: null,
            MaxTotalDurationMinutes: null,
            RequireTicketAvailability: false);
}
