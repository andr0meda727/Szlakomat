using Szlakomat.TripRecommendation.Application;
using Szlakomat.TripRecommendation.Application.Planning;

namespace Szlakomat.TripRecommendation.Infrastructure;

/// <summary>
/// Mock adaptera <see cref="IAttractionCatalogReader"/> zasilany danymi Wawelu
/// (wyciągniętymi z KrakowSeedData).
///
/// Używany w środowisku deweloperskim — docelowo zastąpiony adapterem
/// czytającym z InMemoryProductTypeRepository / ICatalogEntryRepository.
/// </summary>
internal sealed class MockAttractionCatalogReader : IAttractionCatalogReader
{
    // -----------------------------------------------------------------------
    // Statyczny katalog — odzwierciedla dane z WawelExhibitionsSeed
    // -----------------------------------------------------------------------
    private static readonly IReadOnlyDictionary<string, AttractionMetadata> Catalog =
        BuildCatalog();

    public Task<IReadOnlyDictionary<string, AttractionMetadata>> GetMetadataAsync(
        IReadOnlyCollection<string> attractionIds,
        CancellationToken ct)
    {
        var result = attractionIds
            .Where(id => Catalog.ContainsKey(id))
            .ToDictionary(id => id, id => Catalog[id]);

        return Task.FromResult<IReadOnlyDictionary<string, AttractionMetadata>>(result);
    }

    // -----------------------------------------------------------------------
    // Dane katalogu
    // -----------------------------------------------------------------------
    private static Dictionary<string, AttractionMetadata> BuildCatalog()
    {
        var entries = new[]
        {
            // ── Wystawy (exhibitions) ────────────────────────────────────
            Entry("wawel-state-rooms",
                  "Komnaty Królewskie",
                  90, AttractionCategory.Heritage, AttractionCategory.Museum),

            Entry("wawel-royal-private-apartments",
                  "Prywatne Apartamenty Królewskie",
                  75, AttractionCategory.Heritage, AttractionCategory.Museum),

            Entry("wawel-crown-treasury-armory",
                  "Skarbiec Koronny i Zbrojownia",
                  60, AttractionCategory.Heritage, AttractionCategory.Museum),

            Entry("wawel-lost-wawel",
                  "Wawel Zaginiony",
                  45, AttractionCategory.Heritage, AttractionCategory.Architecture),

            Entry("wawel-oriental-collection",
                  "Kolekcja Wschodnia",
                  50, AttractionCategory.Museum, AttractionCategory.Art),

            Entry("wawel-dragon-den",
                  "Smocza Jama",
                  30, AttractionCategory.Outdoor, AttractionCategory.Entertainment),

            Entry("wawel-cathedral",
                  "Katedra Wawelska",
                  60, AttractionCategory.Religious, AttractionCategory.Heritage),

            Entry("wawel-cathedral-museum",
                  "Muzeum Katedralne",
                  45, AttractionCategory.Museum, AttractionCategory.Religious),

            Entry("wawel-sigismund-tower",
                  "Wieża Zygmuntowska",
                  30, AttractionCategory.Heritage, AttractionCategory.Architecture),

            Entry("wawel-garden",
                  "Ogrody Królewskie",
                  40, AttractionCategory.Outdoor, AttractionCategory.NatureAndPark),

            Entry("wawel-fortifications",
                  "Fortyfikacje Warowne",
                  50, AttractionCategory.Heritage, AttractionCategory.Architecture),

            Entry("wawel-art-of-the-east",
                  "Sztuka Wschodu",
                  45, AttractionCategory.Art, AttractionCategory.Museum),

            Entry("wawel-exhibition-leonardo",
                  "Dama z Gronostajem — ekspozycja",
                  60, AttractionCategory.Art, AttractionCategory.Museum),

            // ── Usługi (services) ─────────────────────────────────────────
            Entry("wawel-audio-guide",
                  "Audioprzewodnik",
                  0, AttractionCategory.Entertainment),

            Entry("wawel-guided-tour-pl",
                  "Wycieczka z przewodnikiem (PL)",
                  90, AttractionCategory.Entertainment),

            Entry("wawel-guided-tour-en",
                  "Guided Tour (EN)",
                  90, AttractionCategory.Entertainment),

            Entry("wawel-photo-permit",
                  "Pozwolenie na fotografowanie",
                  0, AttractionCategory.Entertainment),

            Entry("wawel-virtual-reconstruction",
                  "Wirtualna rekonstrukcja zamku",
                  20, AttractionCategory.Entertainment, AttractionCategory.Museum),
        };

        return entries.ToDictionary(e => e.AttractionId);
    }

    private static AttractionMetadata Entry(
        string id,
        string name,
        int durationMinutes,
        params AttractionCategory[] categories) =>
        new(
            AttractionId:             id,
            DisplayName:              name,
            Categories:               new HashSet<AttractionCategory>(categories),
            EstimatedDurationMinutes: durationMinutes
        );
}
