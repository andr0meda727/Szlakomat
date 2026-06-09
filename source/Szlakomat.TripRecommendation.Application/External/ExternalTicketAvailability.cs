namespace Szlakomat.TripRecommendation.Application.External;

public sealed record ExternalTicketAvailability(
    string AttractionId,
    bool AreTicketsRequired,
    bool AreTicketsAvailable
);
