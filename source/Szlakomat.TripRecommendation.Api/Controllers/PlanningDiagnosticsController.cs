using MediatR;
using Microsoft.AspNetCore.Mvc;
using Szlakomat.TripRecommendation.Api.Contracts;
using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Api.Controllers;

/// <summary>
/// Diagnostic endpoints kept for exercising the earlier normalization and scoring work.
/// They are not the final output of the recommendation group.
/// </summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public sealed class PlanningDiagnosticsController : ControllerBase
{
    [HttpPost("planning/snapshot")]
    public async Task<IActionResult> CreateSnapshot(
        CreatePlanningSnapshotRequest request,
        CorrectedPlanningInputFactory factory,
        CancellationToken cancellationToken)
    {
        var snapshot = await factory.CreateAsync(ToPlanningInput(request), cancellationToken);

        return Ok(new
        {
            snapshot.PlanningSessionId,
            snapshot.UserId,
            snapshot.TravelDate,
            snapshot.TargetCurrency,
            CandidateCount = snapshot.Candidates.Count,
            snapshot.Preferences,
            snapshot.Constraints,
            Candidates = snapshot.Candidates.Select(c => new
            {
                c.AttractionId,
                c.DisplayName,
                Categories = c.Categories.Select(x => x.ToString()),
                c.ScoreNormalized,
                c.PriceInTargetCurrency,
                c.PreferenceNormalized,
                c.AvailabilityRatio,
                c.IsTicketAvailable,
                c.EstimatedDurationMinutes
            })
        });
    }

    [HttpPost("planning/recommendations")]
    public async Task<IActionResult> DetermineWorthVisiting(
        CreatePlanningSnapshotRequest request,
        CorrectedPlanningInputFactory factory,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var snapshot = await factory.CreateAsync(ToPlanningInput(request), cancellationToken);
        var recommendations = await mediator.Send(new DetermineWorthVisiting(snapshot), cancellationToken);

        return Ok(new
        {
            snapshot.PlanningSessionId,
            snapshot.UserId,
            snapshot.TravelDate,
            snapshot.TargetCurrency,
            RecommendationCount = recommendations.Count,
            Recommendations = recommendations
        });
    }

    [HttpPost("normalization/attractions")]
    public async Task<IActionResult> NormalizeAttractions(
        NormalizeRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new NormalizeTripData(
            UserId: request.UserId,
            AttractionIds: request.AttractionIds,
            TravelDate: request.TravelDate,
            TargetCurrency: request.TargetCurrency), cancellationToken);

        return Ok(response);
    }

    private static PlanningInputRequest ToPlanningInput(CreatePlanningSnapshotRequest request)
    {
        var hasConstraints =
            request.MaxBudget.HasValue ||
            request.MaxAttractions.HasValue ||
            request.MaxTotalDurationMinutes.HasValue ||
            request.RequireTicketAvailability;

        return new PlanningInputRequest(
            PlanningSessionId: Guid.NewGuid().ToString(),
            UserId: request.UserId,
            AttractionIds: request.AttractionIds.ToHashSet(),
            TravelDate: request.TravelDate,
            TargetCurrency: request.TargetCurrency,
            Weights: request.Weights is { } w
                ? new ScoringWeights(w.ScoreWeight, w.PreferenceWeight, w.PriceWeight, w.AvailabilityWeight)
                : null,
            PreferredCategories: request.PreferredCategories?.Select(Enum.Parse<AttractionCategory>).ToHashSet(),
            ExcludedCategories: request.ExcludedCategories?.Select(Enum.Parse<AttractionCategory>).ToHashSet(),
            Constraints: hasConstraints
                ? new PlanningConstraints(
                    MaxTotalBudget: request.MaxBudget,
                    MaxAttractions: request.MaxAttractions,
                    MaxTotalDurationMinutes: request.MaxTotalDurationMinutes,
                    RequireTicketAvailability: request.RequireTicketAvailability)
                : null);
    }
}
