namespace TripRecommendation;

public static class NormalizationMath
{
    public static decimal Clamp01(decimal value)
    {
        if (value < 0m) return 0m;
        if (value > 1m) return 1m;
        return value;
    }

    public static decimal Score100To01(decimal score0To100) => Clamp01(score0To100 / 100m);

    public static decimal PreferenceMinus100To01(decimal preferenceMinus100To100) =>
        Clamp01((preferenceMinus100To100 + 100m) / 200m);

    public static decimal AvailabilityToRatio(int available, int total) =>
        total <= 0 ? 0m : Clamp01((decimal)available / total);
}