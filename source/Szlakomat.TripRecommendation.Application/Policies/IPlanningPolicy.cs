using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Policies;

public interface IPlanningPolicy
{
    string Id { get; }
    int Order { get; }
    bool IsEnabledFor(CorrectedPlanningInput input);
    bool AppliesTo(PlanningPolicyContext context);
    Task<IReadOnlyCollection<PlanningPolicyDecision>> EvaluateAsync(
        PlanningPolicyContext context,
        CancellationToken cancellationToken
    );
}
