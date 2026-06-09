using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

internal sealed class MustSeePolicy : IPlanningPolicy
{
    public string Id => "must-see";

    public int Order => 300;

    public bool IsEnabledFor(CorrectedPlanningInput input)
    {
        return input.Preferences.MustSeeAttractionIds.Count > 0;
    }

    public bool AppliesTo(PlanningPolicyContext context)
    {
        return context.Input.Preferences.MustSeeAttractionIds.Contains(
            context.Candidate.AttractionId,
            StringComparer.Ordinal
        );
    }

    public Task<IReadOnlyCollection<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyCollection<PlanningPolicyDecision> decisions =
        [
            PlanningPolicyDecision.Require("User marked this candidate as must-see."),
            PlanningPolicyDecision.ScoreAdjustment(0.15m, "Must-see candidates receive a priority boost.")
        ];

        return Task.FromResult(decisions);
    }
}
