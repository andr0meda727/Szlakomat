using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

internal sealed class PreferredCategoryPolicy : IPlanningPolicy
{
    public string Id => "preferred-category";

    public int Order => 400;

    public bool IsEnabledFor(CorrectedPlanningInput input)
    {
        return input.Preferences.PreferredCategories.Count > 0;
    }

    public bool AppliesTo(PlanningPolicyContext context)
    {
        return context.Candidate.Categories.Count > 0;
    }

    public Task<IReadOnlyCollection<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        var preferred = context.Input.Preferences.PreferredCategories;
        var matchesPreference = context.Candidate.Categories.Any(preferred.Contains);

        if (!matchesPreference)
        {
            return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
                [PlanningPolicyDecision.ScoreAdjustment(-0.05m, "Candidate does not match preferred categories.")]
            );
        }

        return Task.FromResult<IReadOnlyCollection<PlanningPolicyDecision>>(
            [PlanningPolicyDecision.ScoreAdjustment(0.10m, "Candidate matches preferred categories.")]
        );
    }
}
