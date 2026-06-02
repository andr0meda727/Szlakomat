namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public sealed class TicketAvailabilityPolicy : IPlanningPolicy
{
    private const decimal LowAvailabilityThreshold = 0.20m;

    public string Id => "ticket-availability";
    public int Order => 100;

    public ValueTask<IReadOnlyList<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken)
    {
        if (!context.Input.Constraints.RequireTicketAvailability)
        {
            return ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([
                new PlanningPolicyDecision(
                    PlanningPolicyDecisionType.NoOpinion,
                    Id,
                    "TICKET_AVAILABILITY_NOT_REQUIRED",
                    "Ticket availability is not required for this planning session.")
            ]);
        }

        if (!context.Candidate.IsTicketAvailable)
        {
            return ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([
                new PlanningPolicyDecision(
                    PlanningPolicyDecisionType.Reject,
                    Id,
                    "NO_TICKETS_AVAILABLE",
                    "Candidate has no available tickets and ticket availability is required.",
                    IsHardConstraint: true)
            ]);
        }

        if (context.Candidate.AvailabilityRatio <= LowAvailabilityThreshold)
        {
            return ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([
                new PlanningPolicyDecision(
                    PlanningPolicyDecisionType.Warning,
                    Id,
                    "LOW_TICKET_AVAILABILITY",
                    "Candidate has low ticket availability.")
            ]);
        }

        return ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([
            new PlanningPolicyDecision(
                PlanningPolicyDecisionType.NoOpinion,
                Id,
                "TICKETS_AVAILABLE",
                "Candidate has available tickets.")
        ]);
    }
}
