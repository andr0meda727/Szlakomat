using Microsoft.Extensions.DependencyInjection;
using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTripNormalization(this IServiceCollection services)
    {
        // ── Zewnętrzni providerzy danych ──────────────────────────────────
        services.AddSingleton<IScoringProvider,           MockScoringProvider>();
        services.AddSingleton<IPricingProvider,           MockPricingProvider>();
        services.AddSingleton<IUserPreferencesProvider,   MockUserPreferencesProvider>();
        services.AddSingleton<ITicketAvailabilityProvider, MockTicketAvailabilityProvider>();
        services.AddSingleton<ICurrencyExchangeProvider,  MockCurrencyExchangeProvider>();

        // ── Katalog atrakcji ──────────────────────────────────────────────
        services.AddSingleton<IAttractionCatalogReader,   MockAttractionCatalogReader>();

        // ── Fabryka snapshotu planowania ──────────────────────────────────
        services.AddScoped<CorrectedPlanningInputFactory>();

        // ── MediatR (NormalizeTripData — stary flow) ──────────────────────
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<NormalizeTripData>());

        return services;
    }
}