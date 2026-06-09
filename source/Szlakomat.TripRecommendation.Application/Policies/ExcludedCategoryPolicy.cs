using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

internal sealed class ExcludedCategoryPolicy : IPlanningPolicy
{
    public string Id => "excluded-category";

    public int Order => 200;

    public bool IsEnabledFor(CorrectedPlanningInput input)
    {
        return input.Preferences.ExcludedCategories.Count > 0;
    }

    public bool AppliesTo(PlanningPolicyContext context)
    {
        var excluded = context.Input.Preferences.ExcludedCategories;
        return context.Candidate.Categories.Any(excluded.Contains);
    }

    public Task<IReadOnlyCollection<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
            [PlanningPolicyDecision.Reject("Candidate belongs to an excluded category.")]
        );
    }
}
