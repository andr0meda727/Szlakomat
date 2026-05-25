using Szlakomat.TripRecommendation.Application;

namespace Szlakomat.TripRecommendation.Infrastructure;

internal sealed class MockScoringProvider : IScoringProvider
{
    public Task<IReadOnlyDictionary<string, ExternalScoreInput>> GetAsync(IReadOnlyCollection<string> ids, DateOnly date, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<string, ExternalScoreInput>>(
            ids.ToDictionary(x => x, x => new ExternalScoreInput(x, 40m + (Math.Abs(HashCode.Combine(x, date.DayNumber)) % 61))));
}

internal sealed class MockPricingProvider : IPricingProvider
{
    private static readonly string[] Currencies = ["PLN", "EUR", "USD"];

    public Task<IReadOnlyDictionary<string, ExternalPriceInput>> GetAsync(IReadOnlyCollection<string> ids, DateOnly date, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<string, ExternalPriceInput>>(
            ids.ToDictionary(x => x, x =>
            {
                var seed = Math.Abs(HashCode.Combine(x, date.Month, date.Day));
                return new ExternalPriceInput(x, 15m + (seed % 350), Currencies[seed % Currencies.Length]);
            }));
}

internal sealed class MockUserPreferencesProvider : IUserPreferencesProvider
{
    public Task<IReadOnlyDictionary<string, ExternalPreferenceInput>> GetAsync(string userId, IReadOnlyCollection<string> ids, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<string, ExternalPreferenceInput>>(
            ids.ToDictionary(x => x, x => new ExternalPreferenceInput(x, (Math.Abs(HashCode.Combine(userId, x)) % 201) - 100)));
}

internal sealed class MockTicketAvailabilityProvider : ITicketAvailabilityProvider
{
    public Task<IReadOnlyDictionary<string, ExternalTicketAvailabilityInput>> GetAsync(IReadOnlyCollection<string> ids, DateOnly date, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<string, ExternalTicketAvailabilityInput>>(
            ids.ToDictionary(x => x, x =>
            {
                var seed = Math.Abs(HashCode.Combine(x, date.DayNumber, "tickets"));
                var total = 50 + (seed % 451);
                var available = seed % (total + 1);
                return new ExternalTicketAvailabilityInput(x, available, total);
            }));
}

internal sealed class MockCurrencyExchangeProvider : ICurrencyExchangeProvider
{
    private static readonly Dictionary<string, decimal> ToPln = new(StringComparer.OrdinalIgnoreCase)
    {
        ["PLN"] = 1m, ["EUR"] = 4.30m, ["USD"] = 4.00m
    };

    public Task<decimal> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken ct)
    {
        if (!ToPln.TryGetValue(fromCurrency, out var fromToPln))
            throw new InvalidOperationException($"Unsupported currency: {fromCurrency}");
        if (!ToPln.TryGetValue(toCurrency, out var toToPln))
            throw new InvalidOperationException($"Unsupported currency: {toCurrency}");

        return Task.FromResult(fromToPln / toToPln);
    }
}