using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.TripRecommendation.Api.Contracts.Recommendations;
using Szlakomat.TripRecommendation.Api.Mappers;
using Szlakomat.TripRecommendation.Application.Recommendations;

namespace Szlakomat.TripRecommendation.Api.Controllers;

[ApiController]
[Route("api/trip-recommendations")]
public sealed class TripRecommendationsController(ISender sender) : ControllerBase
{
    [HttpPost("plan")]
    public async Task<ActionResult<PlanRecommendationsResponse>> PlanAsync(
        [FromBody] PlanRecommendationsRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.PlanningSessionId))
        {
            return BadRequest("PlanningSessionId is required.");
        }

        var result = await sender.Send(
            new PlanVisitRecommendations(request.PlanningSessionId),
            cancellationToken
        );

        return Ok(result.ToResponse());
    }
}
