using Microsoft.Extensions.DependencyInjection;
using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Application.Recommendations.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;
using Szlakomat.TripRecommendation.Application.Recommendations.Selection;

namespace Szlakomat.TripRecommendation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTripRecommendationModule(this IServiceCollection services)
    {
        services.AddTripNormalization();

        services.AddScoped<IBaseWorthScorer, WorthVisitingBaseScorer>();
        services.AddScoped<PlanningRuleEngine>();
        services.AddScoped<VisitPlanSelector>();
        services.AddScoped<IPlanningPolicy, TicketAvailabilityPolicy>();
        services.AddScoped<IPlanningPolicyFactory, DefaultPlanningPolicyFactory>();

        return services;
    }

    public static IServiceCollection AddTripNormalization(this IServiceCollection services)
    {
        services.AddSingleton<IScoringProvider, MockScoringProvider>();
        services.AddSingleton<IPricingProvider, MockPricingProvider>();
        services.AddSingleton<IUserPreferencesProvider, MockUserPreferencesProvider>();
        services.AddSingleton<ITicketAvailabilityProvider, MockTicketAvailabilityProvider>();
        services.AddSingleton<ICurrencyExchangeProvider, MockCurrencyExchangeProvider>();

        services.AddSingleton<IAttractionCatalogReader, MockAttractionCatalogReader>();
        services.AddScoped<CorrectedPlanningInputFactory>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<TripRecommendationModule>());

        return services;
    }
}
