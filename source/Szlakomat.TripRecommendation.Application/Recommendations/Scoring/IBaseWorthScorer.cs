using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

public interface IBaseWorthScorer
{
    IReadOnlyDictionary<string, BaseWorthScore> Score(CorrectedPlanningInput input);
}
