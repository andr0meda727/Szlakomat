namespace Szlakomat.TripRecommendation.Application;

public sealed record ExternalScoreInput(string AttractionId, decimal RawScore0To100);
public sealed record ExternalPriceInput(string AttractionId, decimal Amount, string CurrencyCode);
public sealed record ExternalPreferenceInput(string AttractionId, decimal RawPreferenceMinus100To100);
public sealed record ExternalTicketAvailabilityInput(string AttractionId, int AvailableTickets, int TotalTickets);

public interface IScoringProvider
{
    Task<IReadOnlyDictionary<string, ExternalScoreInput>> GetAsync(IReadOnlyCollection<string> ids, DateOnly date, CancellationToken ct);
}

public interface IPricingProvider
{
    Task<IReadOnlyDictionary<string, ExternalPriceInput>> GetAsync(IReadOnlyCollection<string> ids, DateOnly date, CancellationToken ct);
}

public interface IUserPreferencesProvider
{
    Task<IReadOnlyDictionary<string, ExternalPreferenceInput>> GetAsync(string userId, IReadOnlyCollection<string> ids, CancellationToken ct);
}

public interface ITicketAvailabilityProvider
{
    Task<IReadOnlyDictionary<string, ExternalTicketAvailabilityInput>> GetAsync(IReadOnlyCollection<string> ids, DateOnly date, CancellationToken ct);
}

public interface ICurrencyExchangeProvider
{
    Task<decimal> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken ct);
}