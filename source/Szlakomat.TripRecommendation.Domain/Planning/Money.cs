namespace Szlakomat.TripRecommendation.Domain.Planning;

public readonly record struct Money(decimal Amount, string CurrencyCode)
{
    public static Money Zero(string currencyCode) => new(0m, currencyCode);
}
