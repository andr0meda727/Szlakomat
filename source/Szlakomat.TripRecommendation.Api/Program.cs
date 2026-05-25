using MediatR;
using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTripNormalization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/normalization/attractions", async (
    NormalizeRequest request,
    IMediator mediator,
    CancellationToken ct) =>
{
    var response = await mediator.Send(new NormalizeTripData(
        UserId: request.UserId,
        AttractionIds: request.AttractionIds,
        TravelDate: request.TravelDate,
        TargetCurrency: request.TargetCurrency), ct);

    return Results.Ok(response);
});

app.Run();

public sealed record NormalizeRequest(
    string UserId,
    IReadOnlySet<string> AttractionIds,
    DateOnly TravelDate,
    string TargetCurrency
    );