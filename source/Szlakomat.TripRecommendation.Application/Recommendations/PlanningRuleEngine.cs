using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

internal sealed class PlanningRuleEngine : IPlanningRuleEngine
{
    public async Task<IReadOnlyCollection<CandidatePolicyEvaluation>> EvaluateAsync(
        CorrectedPlanningInput input,
        IReadOnlyCollection<IPlanningPolicy> policies,
        CancellationToken cancellationToken
    )
    {
        var evaluations = new List<CandidatePolicyEvaluation>();

        foreach (var candidate in input.Candidates)
        {
            var baseScore = Math.Clamp(candidate.ScoreNormalized, 0m, 1m);
            var context = new PlanningPolicyContext(input, candidate, baseScore);
            var decisions = new List<PlanningPolicyDecision>();

            foreach (var policy in policies)
            {
                if (!policy.AppliesTo(context))
                {
                    continue;
                }

                var policyDecisions = await policy.EvaluateAsync(context, cancellationToken);
                decisions.AddRange(policyDecisions);
            }

            var scoreDelta = decisions
                .Where(decision => decision.Type == PlanningPolicyDecisionType.ScoreAdjustment)
                .Sum(decision => decision.ScoreDelta);

            var finalScore = Math.Clamp(baseScore + scoreDelta, 0m, 1m);

            evaluations.Add(new CandidatePolicyEvaluation(
                candidate,
                baseScore,
                finalScore,
                decisions.Any(decision => decision.Type == PlanningPolicyDecisionType.Reject),
                decisions.Any(decision => decision.Type == PlanningPolicyDecisionType.Require),
                decisions
            ));
        }

        return evaluations;
    }
}
