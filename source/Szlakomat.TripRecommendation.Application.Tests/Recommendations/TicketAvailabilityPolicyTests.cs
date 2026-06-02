using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class TicketAvailabilityPolicyTests
{
    [Fact]
    public async Task EvaluateAsync_WhenTicketsAreRequiredAndCandidateHasNoTickets_ReturnsHardReject()
    {
        var candidate = PlanningTestData.Candidate("sold-out", tickets: false, availability: 0m);
        var input = PlanningTestData.Input(
            new PlanningConstraints(null, null, null, true),
            candidate);

        var decisions = await new TicketAvailabilityPolicy().EvaluateAsync(
            Context(input, candidate),
            CancellationToken.None);

        var decision = Assert.Single(decisions);
        Assert.Equal(PlanningPolicyDecisionType.Reject, decision.Type);
        Assert.True(decision.IsHardConstraint);
    }

    [Fact]
    public void AppliesTo_WhenRequireTicketAvailabilityFalse_ReturnsFalse()
    {
        var input = PlanningTestData.Input(PlanningConstraints.None);
        Assert.False(new TicketAvailabilityPolicy().AppliesTo(input));
    }

    [Fact]
    public void AppliesTo_WhenRequireTicketAvailabilityTrue_ReturnsTrue()
    {
        var input = PlanningTestData.Input(new PlanningConstraints(null, null, null, true));
        Assert.True(new TicketAvailabilityPolicy().AppliesTo(input));
    }

    private static PlanningPolicyContext Context(
        CorrectedPlanningInput input,
        ScoringCandidate candidate) =>
        new(input, candidate, new BaseWorthScore(candidate.AttractionId, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m));
}
