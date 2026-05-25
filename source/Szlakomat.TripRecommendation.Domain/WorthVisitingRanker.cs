namespace TripRecommendation;

public static class WorthVisitingRanker
{
    public static IReadOnlyList<WorthVisitingRecommendation> Rank(
        IReadOnlyCollection<WorthVisitingCandidate> candidates,
        WorthVisitingRankingSettings settings)
    {
        ValidateWeights(settings);

        if (candidates.Count == 0)
        {
            return Array.Empty<WorthVisitingRecommendation>();
        }

        var eligible = candidates
            .Where(x => !settings.RequireTicketAvailability || x.IsTicketAvailable)
            .ToArray();

        if (eligible.Length == 0)
        {
            return Array.Empty<WorthVisitingRecommendation>();
        }

        var minPrice = eligible.Min(x => x.PriceInTargetCurrency);
        var maxPrice = eligible.Max(x => x.PriceInTargetCurrency);

        var ranked = eligible
            .Select(x => BuildRecommendation(x, settings, minPrice, maxPrice))
            .OrderByDescending(x => x.WorthScore)
            .ThenBy(x => x.PriceInTargetCurrency)
            .ThenBy(x => x.EstimatedDurationMinutes)
            .ThenBy(x => x.AttractionId, StringComparer.Ordinal)
            .ToArray();

        return ApplyPlanConstraints(ranked, settings);
    }

    private static WorthVisitingRecommendation BuildRecommendation(
        WorthVisitingCandidate candidate,
        WorthVisitingRankingSettings settings,
        decimal minPrice,
        decimal maxPrice)
    {
        var score = NormalizationMath.Clamp01(candidate.ScoreNormalized);
        var preference = NormalizationMath.Clamp01(candidate.PreferenceNormalized);
        var availability = NormalizationMath.Clamp01(candidate.AvailabilityRatio);
        var priceScore = CalculatePriceScore(candidate.PriceInTargetCurrency, minPrice, maxPrice);

        var worthScore =
            settings.ScoreWeight * score +
            settings.PreferenceWeight * preference +
            settings.PriceWeight * priceScore +
            settings.AvailabilityWeight * availability;

        var roundedWorth = decimal.Round(NormalizationMath.Clamp01(worthScore), 4);
        var roundedPrice = decimal.Round(priceScore, 4);
        var roundedScore = decimal.Round(score, 4);
        var roundedPreference = decimal.Round(preference, 4);
        var roundedAvailability = decimal.Round(availability, 4);
        var level = ResolveLevel(roundedWorth);

        return new WorthVisitingRecommendation(
            candidate.AttractionId,
            candidate.DisplayName,
            roundedWorth,
            roundedScore,
            roundedPreference,
            roundedPrice,
            roundedAvailability,
            candidate.PriceInTargetCurrency,
            candidate.TargetCurrency,
            candidate.EstimatedDurationMinutes,
            candidate.IsTicketAvailable,
            level,
            BuildReason(level, roundedWorth, roundedPrice));
    }

    private static IReadOnlyList<WorthVisitingRecommendation> ApplyPlanConstraints(
        IReadOnlyList<WorthVisitingRecommendation> ranked,
        WorthVisitingRankingSettings settings)
    {
        var selected = new List<WorthVisitingRecommendation>(ranked.Count);
        var totalPrice = 0m;
        var totalDuration = 0;

        foreach (var recommendation in ranked)
        {
            if (settings.MaxAttractions is { } maxAttractions && selected.Count >= maxAttractions)
            {
                break;
            }

            var nextPrice = totalPrice + recommendation.PriceInTargetCurrency;
            if (settings.MaxTotalBudget is { } maxBudget && nextPrice > maxBudget)
            {
                continue;
            }

            var nextDuration = totalDuration + recommendation.EstimatedDurationMinutes;
            if (settings.MaxTotalDurationMinutes is { } maxDuration && nextDuration > maxDuration)
            {
                continue;
            }

            selected.Add(recommendation);
            totalPrice = nextPrice;
            totalDuration = nextDuration;
        }

        return selected;
    }

    private static decimal CalculatePriceScore(decimal price, decimal minPrice, decimal maxPrice)
    {
        if (maxPrice <= minPrice)
        {
            return 1m;
        }

        var normalizedPrice = (price - minPrice) / (maxPrice - minPrice);
        return 1m - NormalizationMath.Clamp01(normalizedPrice);
    }

    private static WorthVisitingLevel ResolveLevel(decimal worthScore)
    {
        if (worthScore >= 0.85m) return WorthVisitingLevel.DefinitelyWorthVisiting;
        if (worthScore >= 0.65m) return WorthVisitingLevel.WorthVisiting;
        if (worthScore >= 0.45m) return WorthVisitingLevel.Optional;
        return WorthVisitingLevel.Skip;
    }

    private static string BuildReason(WorthVisitingLevel level, decimal worthScore, decimal priceScore) =>
        level switch
        {
            WorthVisitingLevel.DefinitelyWorthVisiting =>
                $"Very strong match with score {worthScore:0.00} and price attractiveness {priceScore:0.00}.",
            WorthVisitingLevel.WorthVisiting =>
                $"Good match with score {worthScore:0.00} and price attractiveness {priceScore:0.00}.",
            WorthVisitingLevel.Optional =>
                $"Moderate match with score {worthScore:0.00} and price attractiveness {priceScore:0.00}.",
            _ =>
                $"Weak match with score {worthScore:0.00} and price attractiveness {priceScore:0.00}."
        };

    private static void ValidateWeights(WorthVisitingRankingSettings settings)
    {
        const decimal tolerance = 0.001m;
        if (Math.Abs(settings.WeightSum - 1m) > tolerance)
        {
            throw new ArgumentException($"Ranking weights must sum to 1.0, got {settings.WeightSum:F4}.", nameof(settings));
        }
    }
}
