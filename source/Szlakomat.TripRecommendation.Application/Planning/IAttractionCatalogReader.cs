using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Application;

/// <summary>
/// Port (interfejs wyjściowy) do odczytu metadanych atrakcji z katalogu Szlakomatu.
/// Implementacja (adapter) żyje w Infrastructure i korzysta z InMemoryProductTypeRepository / ICatalogEntryRepository.
/// </summary>
public interface IAttractionCatalogReader
{
    /// <summary>
    /// Zwraca metadane dla podanych identyfikatorów atrakcji.
    /// Atrakcje nieznane katalogowi są pomijane (brak klucza w słowniku).
    /// </summary>
    Task<IReadOnlyDictionary<string, AttractionMetadata>> GetMetadataAsync(
        IReadOnlyCollection<string> attractionIds,
        CancellationToken ct);
}

/// <summary>
/// Płaskie metadane atrakcji odczytane z katalogu — bez danych cenowych/scoringowych.
/// </summary>
public sealed record AttractionMetadata(
    string AttractionId,
    string DisplayName,
    IReadOnlySet<AttractionCategory> Categories,

    /// <summary>
    /// Szacowany czas zwiedzania w minutach.
    /// Domyślna wartość gdy katalog nie zawiera tej informacji: 60.
    /// </summary>
    int EstimatedDurationMinutes
);
