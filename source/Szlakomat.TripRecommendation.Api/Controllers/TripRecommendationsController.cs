using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.TripRecommendation.Api.Contracts.Recommendations;
using Szlakomat.TripRecommendation.Api.Mappers;
using Szlakomat.TripRecommendation.Application.Recommendations;

namespace Szlakomat.TripRecommendation.Api.Controllers;

[ApiController]
[Route("api/trip-recommendations")]
[Produces("application/json")]
public sealed class TripRecommendationsController(ISender mediator) : ControllerBase
{
    [HttpPost("plan")]
    [ProducesResponseType(typeof(PlanRecommendationsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlanRecommendationsResponse>> Plan(
        PlanRecommendationsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new PlanVisitRecommendations(request.PlanningSessionId),
            cancellationToken);

        return Ok(RecommendationMapper.ToResponse(result));
    }
}
