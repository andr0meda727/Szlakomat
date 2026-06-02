using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public interface IPlanningPolicy
{
    string Id { get; }
    int Order { get; }

    bool AppliesTo(CorrectedPlanningInput input);

    ValueTask<IReadOnlyList<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken);
}
