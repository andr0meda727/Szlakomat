using MediatR;
using Szlakomat.TripRecommendation.Application.Recommendations.Selection;

namespace Szlakomat.TripRecommendation.Application.Recommendations;

public sealed record PlanVisitRecommendations(string PlanningSessionId)
    : IRequest<VisitPlanResult>;
