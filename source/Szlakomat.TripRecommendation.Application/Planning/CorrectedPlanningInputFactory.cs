using Szlakomat.TripRecommendation.Application.Catalog;
using Szlakomat.TripRecommendation.Application.Normalization;
using Szlakomat.TripRecommendation.Application.Sessions;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Planning;

internal sealed class CorrectedPlanningInputFactory(
    IPlanningSessionReader planningSessionReader,
    IPlanningInputNormalizer normalizer,
    IAttractionCatalogReader catalogReader
) : ICorrectedPlanningInputFactory
{
    public async Task<CorrectedPlanningInput> CreateAsync(
        string planningSessionId,
        CancellationToken cancellationToken
    )
    {
        var session = await planningSessionReader.GetAsync(planningSessionId, cancellationToken);
        var normalized = await normalizer.NormalizeAsync(session, cancellationToken);
        var catalog = await catalogReader.GetAsync(session, cancellationToken);

        var candidates = session.AttractionIds
            .Distinct(StringComparer.Ordinal)
            .Select(attractionId => CreateCandidate(attractionId, normalized, catalog, session.TargetCurrency))
            .ToArray();

        return new CorrectedPlanningInput(
            session.PlanningSessionId,
            session.UserId,
            session.TravelPeriod,
            session.TargetCurrency,
            candidates,
            session.Preferences,
            session.Constraints
        );
    }

    private static PlanningCandidate CreateCandidate(
        string attractionId,
        IReadOnlyDictionary<string, NormalizedAttractionData> normalized,
        IReadOnlyDictionary<string, AttractionCatalogItem> catalog,
        string targetCurrency
    )
    {
        normalized.TryGetValue(attractionId, out var normalizedData);
        catalog.TryGetValue(attractionId, out var catalogItem);

        return new PlanningCandidate(
            attractionId,
            catalogItem?.DisplayName ?? attractionId,
            catalogItem?.Categories ?? Array.Empty<AttractionCategory>(),
            normalizedData?.ScoreNormalized ?? 0m,
            normalizedData?.Price ?? Money.Zero(targetCurrency),
            catalogItem?.EstimatedDurationMinutes ?? 0,
            normalizedData?.TicketAvailability ?? new TicketAvailability(false, true)
        );
    }
}
