using Szlakomat.TripRecommendation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTripRecommendationModule();

var app = builder.Build();
app.MapControllers();
app.Run();
