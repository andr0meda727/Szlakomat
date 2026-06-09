using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

internal sealed class TicketAvailabilityPolicy : IPlanningPolicy
{
    public string Id => "ticket-availability";

    public int Order => 100;

    public bool IsEnabledFor(CorrectedPlanningInput input)
    {
        return input.Candidates.Any(c => c.TicketAvailability.AreTicketsRequired);
    }

    public bool AppliesTo(PlanningPolicyContext context)
    {
        return context.Candidate.TicketAvailability.AreTicketsRequired;
    }

    public Task<IReadOnlyCollection<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        if (!context.Candidate.TicketAvailability.AreTicketsAvailable)
        {
            return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
                [PlanningPolicyDecision.Reject("Ticket is required but not available.")]
            );
        }

        return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
            [PlanningPolicyDecision.NoOpinion("Ticket is required and available.")]
        );
    }
}
