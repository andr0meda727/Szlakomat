using Szlakomat.TripRecommendation.Application.Sessions;

namespace Szlakomat.TripRecommendation.Application.External;

public interface IExternalTicketAvailabilityProvider
{
    Task<IReadOnlyDictionary<string, ExternalTicketAvailability>> GetAsync(
        PlanningSessionSnapshot session,
        CancellationToken cancellationToken
    );
}
