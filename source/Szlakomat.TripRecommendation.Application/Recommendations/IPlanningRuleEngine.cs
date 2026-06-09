using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

public interface IPlanningRuleEngine
{
    Task<IReadOnlyCollection<CandidatePolicyEvaluation>> EvaluateAsync(
        CorrectedPlanningInput input,
        IReadOnlyCollection<IPlanningPolicy> policies,
        CancellationToken cancellationToken
    );
}
