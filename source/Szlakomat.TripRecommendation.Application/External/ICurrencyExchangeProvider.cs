namespace Szlakomat.TripRecommendation.Application.External;

public interface ICurrencyExchangeProvider
{
    Task<decimal> GetRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken);
}
