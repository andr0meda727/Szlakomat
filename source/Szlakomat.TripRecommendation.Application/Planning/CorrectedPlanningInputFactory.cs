using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application.Planning;

/// <summary>
/// Fabryka budująca <see cref="CorrectedPlanningInput"/> ze źródeł zewnętrznych.
///
/// Odpowiedzialności:
/// 1. Równoległy odczyt danych z 5 providerów (scoring, pricing, preferences, tickets, fx) + katalogu.
/// 2. Walidacja i korekta wartości (fallbacki gdy provider nie zwrócił danych dla id).
/// 3. Mapowanie surowych wartości zewnętrznych → znormalizowane pola ScoringCandidate.
/// 4. Walidacja ScoringWeights (suma = 1.0).
/// 5. Odfiltrowywanie atrakcji nieznanych katalogowi (brak metadanych = brak kandydata).
/// </summary>
public sealed class CorrectedPlanningInputFactory(
    IScoringProvider scoring,
    IPricingProvider pricing,
    IUserPreferencesProvider preferences,
    ITicketAvailabilityProvider tickets,
    ICurrencyExchangeProvider fx,
    IAttractionCatalogReader catalog)
{
    // -----------------------------------------------------------------------
    // Stałe fallback
    // -----------------------------------------------------------------------
    private const decimal FallbackScore = 50m;
    private const decimal FallbackPrice = 0m;
    private const decimal FallbackPreference = 0m;
    private const int DefaultDurationMinutes = 60;

    // -----------------------------------------------------------------------
    // Publiczne API
    // -----------------------------------------------------------------------

    /// <summary>
    /// Tworzy <see cref="CorrectedPlanningInput"/> dla podanego żądania planowania.
    /// Wywołuje wszystkich providerów równolegle, następnie składa wynik.
    /// </summary>
    public async Task<CorrectedPlanningInput> CreateAsync(
        PlanningInputRequest request,
        CancellationToken ct)
    {
        ValidateWeights(request.Weights);

        if (request.AttractionIds.Count == 0)
            return BuildEmpty(request);

        var ids = request.AttractionIds.ToArray();

        // ── Równoległy odczyt ──────────────────────────────────────────────
        var scoreTask    = scoring.GetAsync(ids, request.TravelDate, ct);
        var priceTask    = pricing.GetAsync(ids, request.TravelDate, ct);
        var prefTask     = preferences.GetAsync(request.UserId, ids, ct);
        var ticketTask   = tickets.GetAsync(ids, request.TravelDate, ct);
        var metadataTask = catalog.GetMetadataAsync(ids, ct);

        await Task.WhenAll(scoreTask, priceTask, prefTask, ticketTask, metadataTask);

        var scores    = scoreTask.Result;
        var prices    = priceTask.Result;
        var prefs     = prefTask.Result;
        var ticketMap = ticketTask.Result;
        var metadata  = metadataTask.Result;

        // ── Buduj kandydatów ───────────────────────────────────────────────
        var candidates = new List<ScoringCandidate>(ids.Length);

        foreach (var id in ids)
        {
            // Atrakcja nieznana katalogowi — pomijamy (nie możemy jej sensownie przedstawić)
            if (!metadata.TryGetValue(id, out var meta))
                continue;

            var scoreRaw = scores.TryGetValue(id, out var s)
                ? s.RawScore0To100
                : FallbackScore;

            var priceRaw = prices.TryGetValue(id, out var p)
                ? p
                : new ExternalPriceInput(id, FallbackPrice, request.TargetCurrency);

            var prefRaw = prefs.TryGetValue(id, out var pr)
                ? pr.RawPreferenceMinus100To100
                : FallbackPreference;

            var tk = ticketMap.TryGetValue(id, out var t)
                ? t
                : new ExternalTicketAvailabilityInput(id, 0, 0);

            var convertedPrice = await ConvertPriceAsync(
                priceRaw.Amount, priceRaw.CurrencyCode, request.TargetCurrency, ct);

            candidates.Add(new ScoringCandidate(
                AttractionId:            id,
                DisplayName:             meta.DisplayName,
                Categories:              meta.Categories,
                ScoreNormalized:         NormalizationMath.Score100To01(scoreRaw),
                PriceInTargetCurrency:   decimal.Round(convertedPrice, 2),
                PreferenceNormalized:    NormalizationMath.PreferenceMinus100To01(prefRaw),
                AvailabilityRatio:       decimal.Round(
                                             NormalizationMath.AvailabilityToRatio(tk.AvailableTickets, tk.TotalTickets), 4),
                IsTicketAvailable:       tk.AvailableTickets > 0,
                EstimatedDurationMinutes: meta.EstimatedDurationMinutes > 0
                                             ? meta.EstimatedDurationMinutes
                                             : DefaultDurationMinutes
            ));
        }

        return new CorrectedPlanningInput(
            PlanningSessionId: request.PlanningSessionId,
            UserId:            request.UserId,
            TravelDate:        request.TravelDate,
            TargetCurrency:    request.TargetCurrency.ToUpperInvariant(),
            Candidates:        candidates,
            Preferences:       BuildPreferences(request),
            Constraints:       request.Constraints ?? PlanningConstraints.None
        );
    }

    // -----------------------------------------------------------------------
    // Prywatne helpery
    // -----------------------------------------------------------------------

    private static UserPlanningPreferences BuildPreferences(PlanningInputRequest request) =>
        new(
            PreferredCategories: request.PreferredCategories ?? (IReadOnlySet<AttractionCategory>)new HashSet<AttractionCategory>(),
            ExcludedCategories:  request.ExcludedCategories  ?? (IReadOnlySet<AttractionCategory>)new HashSet<AttractionCategory>(),
            Weights:             request.Weights             ?? ScoringWeights.Default
        );

    private async Task<decimal> ConvertPriceAsync(
        decimal amount, string from, string to, CancellationToken ct)
    {
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
            return amount;

        var rate = await fx.GetRateAsync(
            from.ToUpperInvariant(),
            to.ToUpperInvariant(),
            ct);

        return amount * rate;
    }

    private static CorrectedPlanningInput BuildEmpty(PlanningInputRequest request) =>
        new(
            PlanningSessionId: request.PlanningSessionId,
            UserId:            request.UserId,
            TravelDate:        request.TravelDate,
            TargetCurrency:    request.TargetCurrency.ToUpperInvariant(),
            Candidates:        Array.Empty<ScoringCandidate>(),
            Preferences:       BuildPreferences(request),
            Constraints:       request.Constraints ?? PlanningConstraints.None
        );

    private static void ValidateWeights(ScoringWeights? weights)
    {
        if (weights is null) return;

        const decimal tolerance = 0.001m;
        if (Math.Abs(weights.Sum - 1m) > tolerance)
            throw new ArgumentException(
                $"ScoringWeights must sum to 1.0, got {weights.Sum:F4}. " +
                $"(Score={weights.ScoreWeight}, Preference={weights.PreferenceWeight}, " +
                $"Price={weights.PriceWeight}, Availability={weights.AvailabilityWeight})",
                nameof(weights));
    }
}
