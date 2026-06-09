using Microsoft.Extensions.DependencyInjection;
using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Application.Normalization;
using Szlakomat.TripRecommendation.Application.Planning;
using Szlakomat.TripRecommendation.Application.Policies;
using Szlakomat.TripRecommendation.Application.Recommendations;
using Szlakomat.TripRecommendation.Application.Selection;

namespace Szlakomat.TripRecommendation.Infrastructure;

public static class TripRecommendationServiceExtensions
{
    public static void AddTripRecommendationModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<TripRecommendationModule>());

        services.AddScoped<IPlanningInputNormalizer, PlanningInputNormalizer>();
        services.AddScoped<ICorrectedPlanningInputFactory, CorrectedPlanningInputFactory>();

        services.AddScoped<IPlanningPolicy, TicketAvailabilityPolicy>();
        services.AddScoped<IPlanningPolicy, ExcludedCategoryPolicy>();
        services.AddScoped<IPlanningPolicy, MustSeePolicy>();
        services.AddScoped<IPlanningPolicy, PreferredCategoryPolicy>();
        services.AddScoped<IPlanningPolicy, BudgetPolicy>();
        services.AddScoped<IPlanningPolicyFactory, DefaultPlanningPolicyFactory>();

        services.AddScoped<IPlanningRuleEngine, PlanningRuleEngine>();
        services.AddScoped<IVisitPlanSelector, VisitPlanSelector>();
    }
}
