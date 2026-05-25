using MediatR;
using Szlakomat.TripRecommendation.Api.Contracts;
using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTripNormalization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/planning/snapshot", async (
    CreatePlanningSnapshotRequest request,
    CorrectedPlanningInputFactory factory,
    CancellationToken ct) =>
{
    var snapshot = await factory.CreateAsync(ToPlanningInput(request), ct);

    return Results.Ok(new
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
            Categories             = c.Categories.Select(x => x.ToString()),
            c.ScoreNormalized,
            c.PriceInTargetCurrency,
            c.PreferenceNormalized,
            c.AvailabilityRatio,
            c.IsTicketAvailable,
            c.EstimatedDurationMinutes
        })
    });
});

app.MapPost("/api/planning/recommendations", async (
    CreatePlanningSnapshotRequest request,
    CorrectedPlanningInputFactory factory,
    IMediator mediator,
    CancellationToken ct) =>
{
    var snapshot = await factory.CreateAsync(ToPlanningInput(request), ct);
    var recommendations = await mediator.Send(new DetermineWorthVisiting(snapshot), ct);

    return Results.Ok(new
    {
        snapshot.PlanningSessionId,
        snapshot.UserId,
        snapshot.TravelDate,
        snapshot.TargetCurrency,
        RecommendationCount = recommendations.Count,
        Recommendations = recommendations
    });
});

app.MapPost("/api/normalization/attractions", async (
    NormalizeRequest request,
    IMediator mediator,
    CancellationToken ct) =>
{
    var response = await mediator.Send(new NormalizeTripData(
        UserId:        request.UserId,
        AttractionIds: request.AttractionIds,
        TravelDate:    request.TravelDate,
        TargetCurrency: request.TargetCurrency), ct);

    return Results.Ok(response);
});

app.Run();

static PlanningInputRequest ToPlanningInput(CreatePlanningSnapshotRequest request)
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
