using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public sealed class ExcludedCategoryPolicy : IPlanningPolicy
{
    public string Id => "excluded-category";
    public int Order => 50;

    public bool AppliesTo(CorrectedPlanningInput input) =>
        input.Preferences.ExcludedCategories.Count > 0;

    public ValueTask<IReadOnlyList<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken)
    {
        var hasExcludedCategory = context.Candidate.Categories
            .Any(context.Input.Preferences.ExcludedCategories.Contains);

        if (hasExcludedCategory)
        {
            return ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([
                new PlanningPolicyDecision(
                    PlanningPolicyDecisionType.Reject,
                    Id,
                    "EXCLUDED_CATEGORY",
                    "Candidate belongs to a category excluded by the user.",
                    IsHardConstraint: true)
            ]);
        }

        return ValueTask.FromResult<IReadOnlyList<PlanningPolicyDecision>>([
            new PlanningPolicyDecision(
                PlanningPolicyDecisionType.NoOpinion,
                Id,
                "CATEGORY_NOT_EXCLUDED",
                "Candidate does not belong to any excluded category.")
        ]);
    }
}
