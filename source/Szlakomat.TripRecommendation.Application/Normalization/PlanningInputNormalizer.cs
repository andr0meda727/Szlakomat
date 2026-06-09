using Szlakomat.TripRecommendation.Application.External;
using Szlakomat.TripRecommendation.Application.Sessions;
using Szlakomat.TripRecommendation.Domain.Planning;

namespace Szlakomat.TripRecommendation.Application.Normalization;

internal sealed class PlanningInputNormalizer(
    IExternalScoringProvider scoringProvider,
    IExternalPricingProvider pricingProvider,
    IExternalTicketAvailabilityProvider ticketAvailabilityProvider,
    ICurrencyExchangeProvider currencyExchangeProvider
) : IPlanningInputNormalizer
{
    public async Task<IReadOnlyDictionary<string, NormalizedAttractionData>> NormalizeAsync(
        PlanningSessionSnapshot session,
        CancellationToken cancellationToken
    )
    {
        var scores = await scoringProvider.GetAsync(session, cancellationToken);
        var prices = await pricingProvider.GetAsync(session, cancellationToken);
        var tickets = await ticketAvailabilityProvider.GetAsync(session, cancellationToken);

        var normalized = new Dictionary<string, NormalizedAttractionData>(StringComparer.Ordinal);

        foreach (var attractionId in session.AttractionIds.Distinct(StringComparer.Ordinal))
        {
            var score = scores.TryGetValue(attractionId, out var externalScore)
                ? NormalizeScore(externalScore.RawScore0To100)
                : 0m;

            var price = prices.TryGetValue(attractionId, out var externalPrice)
                ? await NormalizePriceAsync(externalPrice, session.TargetCurrency, cancellationToken)
                : Money.Zero(session.TargetCurrency);

            var ticketAvailability = tickets.TryGetValue(attractionId, out var externalTickets)
                ? new TicketAvailability(externalTickets.AreTicketsRequired, externalTickets.AreTicketsAvailable)
                : new TicketAvailability(AreTicketsRequired: false, AreTicketsAvailable: true);

            normalized[attractionId] = new NormalizedAttractionData(
                attractionId,
                score,
                price,
                ticketAvailability
            );
        }

        return normalized;
    }

    private static decimal NormalizeScore(decimal rawScore0To100)
    {
        return Math.Clamp(rawScore0To100 / 100m, 0m, 1m);
    }

    private async Task<Money> NormalizePriceAsync(
        ExternalAttractionPrice price,
        string targetCurrency,
        CancellationToken cancellationToken
    )
    {
        if (string.Equals(price.CurrencyCode, targetCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return new Money(price.Amount, targetCurrency);
        }

        var rate = await currencyExchangeProvider.GetRateAsync(
            price.CurrencyCode,
            targetCurrency,
            cancellationToken
        );

        return new Money(decimal.Round(price.Amount * rate, 2), targetCurrency);
    }
}
