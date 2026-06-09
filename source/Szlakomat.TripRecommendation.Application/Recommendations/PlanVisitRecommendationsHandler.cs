using MediatR;
using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Application.Selection;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

internal sealed class PlanVisitRecommendationsHandler(
    ICorrectedPlanningInputFactory correctedPlanningInputFactory,
    IPlanningPolicyFactory planningPolicyFactory,
    IPlanningRuleEngine planningRuleEngine,
    IVisitPlanSelector visitPlanSelector
) : IRequestHandler<PlanVisitRecommendations, VisitPlanResult>
{
    public async Task<VisitPlanResult> Handle(
        PlanVisitRecommendations request,
        CancellationToken cancellationToken
    )
    {
        var input = await correctedPlanningInputFactory.CreateAsync(
            request.PlanningSessionId,
            cancellationToken
        );

        var policies = planningPolicyFactory.CreatePolicies(input);
        var evaluations = await planningRuleEngine.EvaluateAsync(input, policies, cancellationToken);

        return visitPlanSelector.Select(input, evaluations);
    }
}
