namespace Szlakomat.TripRecommendation.Domain.Planning;

public sealed record TravelPeriod(DateOnly From, DateOnly To)
{
    public TravelPeriod
    {
        if (To < From)
        {
            throw new ArgumentException("Travel period end cannot be earlier than start.");
        }
    }
}
