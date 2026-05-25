using MediatR;
using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application;

internal sealed class NormalizeTripDataHandler(
    IScoringProvider scoring,
    IPricingProvider pricing,
    IUserPreferencesProvider preferences,
    ITicketAvailabilityProvider tickets,
    ICurrencyExchangeProvider fx)
    : IRequestHandler<NormalizeTripData, IReadOnlyList<NormalizedAttractionData>>
{
    public async Task<IReadOnlyList<NormalizedAttractionData>> Handle(NormalizeTripData request, CancellationToken ct)
    {
        if (request.AttractionIds.Count == 0) return Array.Empty<NormalizedAttractionData>();

        var ids = request.AttractionIds.ToArray();

        var scoreTask = scoring.GetAsync(ids, request.TravelDate, ct);
        var priceTask = pricing.GetAsync(ids, request.TravelDate, ct);
        var prefTask = preferences.GetAsync(request.UserId, ids, ct);
        var ticketTask = tickets.GetAsync(ids, request.TravelDate, ct);

        await Task.WhenAll(scoreTask, priceTask, prefTask, ticketTask);

        var scores = scoreTask.Result;
        var prices = priceTask.Result;
        var prefs = prefTask.Result;
        var tickets1 = ticketTask.Result;

        var output = new List<NormalizedAttractionData>(ids.Length);

        foreach (var id in ids)
        {
            var scoreRaw = scores.TryGetValue(id, out var s) ? s.RawScore0To100 : 50m;
            var priceRaw = prices.TryGetValue(id, out var p) ? p : new ExternalPriceInput(id, 0m, request.TargetCurrency);
            var prefRaw = prefs.TryGetValue(id, out var pr) ? pr.RawPreferenceMinus100To100 : 0m;
            var tk = tickets1.TryGetValue(id, out var t) ? t : new ExternalTicketAvailabilityInput(id, 0, 0);

            var convertedPrice = await ConvertAsync(priceRaw.Amount, priceRaw.CurrencyCode, request.TargetCurrency, ct);

            output.Add(new NormalizedAttractionData(
                AttractionId: id,
                ScoreNormalized: NormalizationMath.Score100To01(scoreRaw),
                PriceInTargetCurrency: decimal.Round(convertedPrice, 2),
                TargetCurrency: request.TargetCurrency.ToUpperInvariant(),
                PreferenceNormalized: NormalizationMath.PreferenceMinus100To01(prefRaw),
                AvailabilityRatio: decimal.Round(NormalizationMath.AvailabilityToRatio(tk.AvailableTickets, tk.TotalTickets), 4),
                IsTicketAvailable: tk.AvailableTickets > 0
            ));
        }

        return output;
    }

    private async Task<decimal> ConvertAsync(decimal amount, string from, string to, CancellationToken ct)
    {
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase)) return amount;
        var rate = await fx.GetRateAsync(from.ToUpperInvariant(), to.ToUpperInvariant(), ct);
        return amount * rate;
    }
}