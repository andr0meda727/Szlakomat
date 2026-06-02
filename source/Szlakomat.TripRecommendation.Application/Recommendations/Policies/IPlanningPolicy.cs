namespace Szlakomat.TripRecommendation.Application.Recommendations.Policies;

public interface IPlanningPolicy
{
    string Id { get; }
    int Order { get; }

    ValueTask<IReadOnlyList<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken);
}
