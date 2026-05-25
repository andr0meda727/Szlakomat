namespace Szlakomat.TripRecommendation.Application.Planning;

/// <summary>
/// Znormalizowany snapshot danych wejściowych do silnika scoringu.
/// Tworzony raz per sesja planowania — zawiera kandydatów, preferencje użytkownika
/// oraz ograniczenia (budżet, czas, dostępność biletów).
/// Niezmienny (immutable) — wszystkie późniejsze etapy pipeline'u tylko go czytają.
/// </summary>
public sealed record CorrectedPlanningInput(
    /// <summary>Identyfikator sesji planowania (np. GUID generowany przez API).</summary>
    string PlanningSessionId,

    /// <summary>Identyfikator użytkownika — potrzebny do odczytu preferencji.</summary>
    string UserId,

    /// <summary>Data podróży — wpływa na ceny, dostępność i scoring sezonowy.</summary>
    DateOnly TravelDate,

    /// <summary>Waluta docelowa — wszystkie ceny wyrażone w tej walucie.</summary>
    string TargetCurrency,

    /// <summary>Kandydaci do oceny — atrakcje ze znormalizowanymi danymi zewnętrznymi.</summary>
    IReadOnlyList<ScoringCandidate> Candidates,

    /// <summary>Preferencje użytkownika przekrojowo (poza per-atrakcją).</summary>
    UserPlanningPreferences Preferences,

    /// <summary>Ograniczenia twardych — kandydaci niespełniający ich są wykluczani przed scoringiem.</summary>
    PlanningConstraints Constraints
);

/// <summary>
/// Pojedynczy kandydat do oceny — atrakcja z już przeliczonymi, znormalizowanymi wartościami.
/// Dane pobrane równolegle z zewnętrznych providerów i zmapowane na [0,1].
/// </summary>
public sealed record ScoringCandidate(
    /// <summary>Identyfikator atrakcji (klucz w katalogu Szlakomatu).</summary>
    string AttractionId,

    /// <summary>Czytelna nazwa atrakcji (z katalogu — do logowania i UI).</summary>
    string DisplayName,

    /// <summary>Kategorie atrakcji (np. Museum, Outdoor, Heritage) — używane do filtrów i wag.</summary>
    IReadOnlySet<AttractionCategory> Categories,

    /// <summary>Znormalizowany scoring zewnętrzny [0,1] (wejście: 0–100).</summary>
    decimal ScoreNormalized,

    /// <summary>Cena biletu przeliczona na TargetCurrency.</summary>
    decimal PriceInTargetCurrency,

    /// <summary>Znormalizowana preferencja użytkownika [0,1] (wejście: −100..+100 → 0..1).</summary>
    decimal PreferenceNormalized,

    /// <summary>Stosunek dostępnych biletów do łącznej puli [0,1].</summary>
    decimal AvailabilityRatio,

    /// <summary>Czy w ogóle są dostępne bilety na TravelDate.</summary>
    bool IsTicketAvailable,

    /// <summary>
    /// Szacowany czas zwiedzania w minutach (z katalogu lub domyślny).
    /// Używany przy budowaniu dziennego itinerary z limitem czasu.
    /// </summary>
    int EstimatedDurationMinutes
);

/// <summary>Przekrojowe preferencje użytkownika dotyczące całej wycieczki.</summary>
public sealed record UserPlanningPreferences(
    /// <summary>Kategorie preferowane (może być puste — wtedy brak boostu).</summary>
    IReadOnlySet<AttractionCategory> PreferredCategories,

    /// <summary>Kategorie wykluczone przez użytkownika (hard exclude).</summary>
    IReadOnlySet<AttractionCategory> ExcludedCategories,

    /// <summary>
    /// Wagi użytkownika dla składowych scoringu [0,1].
    /// Pozwalają spersonalizować, co jest ważniejsze: cena, preferencje czy oceny.
    /// </summary>
    ScoringWeights Weights
);

/// <summary>Wagi składowych scoringu — sumują się do 1.0 (walidacja w fabryce).</summary>
public sealed record ScoringWeights(
    decimal ScoreWeight,
    decimal PreferenceWeight,
    decimal PriceWeight,
    decimal AvailabilityWeight
)
{
    public static readonly ScoringWeights Default = new(0.35m, 0.35m, 0.20m, 0.10m);

    public decimal Sum => ScoreWeight + PreferenceWeight + PriceWeight + AvailabilityWeight;
}

/// <summary>
/// Twarde ograniczenia — kandydaci ich niespełniający są odrzucani
/// przed uruchomieniem scoringu (nie wpływają na ranking, tylko na eligibility).
/// </summary>
public sealed record PlanningConstraints(
    /// <summary>Maksymalny budżet na całą wycieczkę w TargetCurrency. null = brak limitu.</summary>
    decimal? MaxTotalBudget,

    /// <summary>Maksymalna liczba atrakcji w planie. null = brak limitu.</summary>
    int? MaxAttractions,

    /// <summary>Łączny dostępny czas w minutach. null = brak limitu.</summary>
    int? MaxTotalDurationMinutes,

    /// <summary>Gdy true — atrakcje bez dostępnych biletów są wykluczane.</summary>
    bool RequireTicketAvailability
)
{
    public static readonly PlanningConstraints None = new(
        MaxTotalBudget: null,
        MaxAttractions: null,
        MaxTotalDurationMinutes: null,
        RequireTicketAvailability: false
    );
}

/// <summary>Kategorie atrakcji używane w filtrach i boostach.</summary>
public enum AttractionCategory
{
    Unknown = 0,
    Museum,
    Heritage,
    Outdoor,
    Religious,
    Entertainment,
    Gastronomy,
    ShoppingAndMarket,
    NatureAndPark,
    Art,
    Architecture
}
