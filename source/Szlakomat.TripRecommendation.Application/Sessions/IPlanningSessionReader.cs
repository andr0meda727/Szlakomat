namespace Szlakomat.TripRecommendation.Application.Sessions;

public interface IPlanningSessionReader
{
    Task<PlanningSessionSnapshot> GetAsync(string planningSessionId, CancellationToken cancellationToken);
}
