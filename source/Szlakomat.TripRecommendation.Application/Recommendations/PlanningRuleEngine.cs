using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;
using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

public sealed class PlanningRuleEngine
{
    public async Task<IReadOnlyList<CandidatePolicyEvaluation>> EvaluateAsync(
        CorrectedPlanningInput input,
        IReadOnlyDictionary<string, BaseWorthScore> baseScores,
        IReadOnlyList<IPlanningPolicy> policies,
        CancellationToken cancellationToken)
    {
        var orderedPolicies = policies
            .OrderBy(policy => policy.Order)
            .ThenBy(policy => policy.Id, StringComparer.Ordinal)
            .ToArray();

        var evaluations = new List<CandidatePolicyEvaluation>(input.Candidates.Count);

        foreach (var candidate in input.Candidates)
        {
            if (!baseScores.TryGetValue(candidate.AttractionId, out var baseScore))
            {
                continue;
            }

            evaluations.Add(await EvaluateCandidateAsync(
                input,
                candidate,
                baseScore,
                orderedPolicies,
                cancellationToken));
        }

        return evaluations;
    }

    private static async Task<CandidatePolicyEvaluation> EvaluateCandidateAsync(
        CorrectedPlanningInput input,
        ScoringCandidate candidate,
        BaseWorthScore baseScore,
        IReadOnlyList<IPlanningPolicy> policies,
        CancellationToken cancellationToken)
    {
        var context = new PlanningPolicyContext(input, candidate, baseScore);
        var decisions = new List<PlanningPolicyDecision>();

        foreach (var policy in policies)
        {
            var policyDecisions = await policy.EvaluateAsync(context, cancellationToken);
            decisions.AddRange(policyDecisions);
        }

        var scoreDelta = decisions
            .Where(decision => decision.Type is PlanningPolicyDecisionType.ScoreAdjustment)
            .Sum(decision => decision.ScoreDelta);

        return new CandidatePolicyEvaluation(
            candidate,
            baseScore,
            NormalizationMath.Clamp01(baseScore.WorthScore + scoreDelta),
            decisions.Any(decision => decision.Type is PlanningPolicyDecisionType.Reject),
            decisions.Any(decision => decision.Type is PlanningPolicyDecisionType.Require),
            decisions);
    }
}
