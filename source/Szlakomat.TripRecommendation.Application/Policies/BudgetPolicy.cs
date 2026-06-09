using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

internal sealed class BudgetPolicy : IPlanningPolicy
{
    public string Id => "budget";

    public int Order => 500;

    public bool IsEnabledFor(CorrectedPlanningInput input)
    {
        return input.Constraints.MaxTotalBudget.HasValue;
    }

    public bool AppliesTo(PlanningPolicyContext context)
    {
        return true;
    }

    public Task<IReadOnlyCollection<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        var maxBudget = context.Input.Constraints.MaxTotalBudget!.Value;

        if (context.Candidate.Price.Amount > maxBudget)
        {
            return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
                [PlanningPolicyDecision.Reject("Candidate price exceeds the whole trip budget.")]
            );
        }

        if (maxBudget <= 0m)
        {
            return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
                [PlanningPolicyDecision.NoOpinion("Budget is not positive.")]
            );
        }

        var budgetShare = context.Candidate.Price.Amount / maxBudget;

        if (budgetShare >= 0.5m)
        {
            return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
                [PlanningPolicyDecision.ScoreAdjustment(-0.10m, "Candidate consumes a large part of the trip budget.")]
            );
        }

        return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
            [PlanningPolicyDecision.NoOpinion("Candidate fits the available budget.")]
        );
    }
}
