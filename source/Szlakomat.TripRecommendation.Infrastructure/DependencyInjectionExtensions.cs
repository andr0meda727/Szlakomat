using Microsoft.Extensions.DependencyInjection;
using Szlakomat.TripRecommendation.Application;

namespace Szlakomat.TripRecommendation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTripNormalization(this IServiceCollection services)
    {
        services.AddSingleton<IScoringProvider, MockScoringProvider>();
        services.AddSingleton<IPricingProvider, MockPricingProvider>();
        services.AddSingleton<IUserPreferencesProvider, MockUserPreferencesProvider>();
        services.AddSingleton<ITicketAvailabilityProvider, MockTicketAvailabilityProvider>();
        services.AddSingleton<ICurrencyExchangeProvider, MockCurrencyExchangeProvider>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<NormalizeTripData>());
        return services;
    }
}