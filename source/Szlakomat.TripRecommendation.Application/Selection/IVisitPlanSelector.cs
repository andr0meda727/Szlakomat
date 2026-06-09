using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Selection;

public interface IVisitPlanSelector
{
    VisitPlanResult Select(
        CorrectedPlanningInput input,
        IReadOnlyCollection<CandidatePolicyEvaluation> evaluations
    );
}
