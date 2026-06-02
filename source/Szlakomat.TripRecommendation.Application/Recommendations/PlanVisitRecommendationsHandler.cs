using MediatR;
using Szlakomat.TripRecommendation.Application.Recommendations.Input;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;
using Szlakomat.TripRecommendation.Application.Recommendations.Selection;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

public sealed class PlanVisitRecommendationsHandler(
    IPlanningInputProvider inputProvider,
    IBaseWorthScorer baseWorthScorer,
    IPlanningPolicyFactory policyFactory,
    PlanningRuleEngine ruleEngine,
    VisitPlanSelector selector)
    : IRequestHandler<PlanVisitRecommendations, VisitPlanResult>
{
    public async Task<VisitPlanResult> Handle(
        PlanVisitRecommendations request,
        CancellationToken cancellationToken)
    {
        var input = await inputProvider.GetAsync(request.PlanningSessionId, cancellationToken);
        var baseScores = baseWorthScorer.Score(input);
        var policies = policyFactory.CreatePolicies(input);
        var evaluations = await ruleEngine.EvaluateAsync(input, baseScores, policies, cancellationToken);

        return selector.Select(input, evaluations);
    }
}
