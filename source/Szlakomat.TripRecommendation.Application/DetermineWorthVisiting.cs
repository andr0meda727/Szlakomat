using MediatR;
using Szlakomat.TripRecommendation.Application.Planning;
using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application;

public sealed record DetermineWorthVisiting(
    CorrectedPlanningInput PlanningInput)
    : IRequest<IReadOnlyList<WorthVisitingRecommendation>>;
