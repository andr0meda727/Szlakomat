namespace Szlakomat.TripRecommendation.Application.Planning;

/// <summary>
/// Żądanie tworzenia <see cref="CorrectedPlanningInput"/>.
/// Przyjmowane z warstwy API lub Application i przekazywane do <see cref="CorrectedPlanningInputFactory"/>.
/// </summary>
public sealed record PlanningInputRequest(
    /// <summary>Unikalny identyfikator sesji planowania (np. Guid.NewGuid().ToString()).</summary>
    string PlanningSessionId,

    string UserId,

    IReadOnlySet<string> AttractionIds,

    DateOnly TravelDate,

    /// <summary>Kod waluty ISO 4217, np. "PLN", "EUR", "USD".</summary>
    string TargetCurrency,

    /// <summary>null → używane są wagi domyślne (<see cref="ScoringWeights.Default"/>).</summary>
    ScoringWeights? Weights = null,

    IReadOnlySet<AttractionCategory>? PreferredCategories = null,
    IReadOnlySet<AttractionCategory>? ExcludedCategories = null,

    /// <summary>null → brak twardych ograniczeń (<see cref="PlanningConstraints.None"/>).</summary>
    PlanningConstraints? Constraints = null
);
