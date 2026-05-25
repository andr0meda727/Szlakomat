using MediatR;
using TripRecommendation;

namespace Szlakomat.TripRecommendation.Application;

public sealed record NormalizeTripData(
    string UserId,
    IReadOnlySet<string> AttractionIds,
    DateOnly TravelDate,
    string TargetCurrency) : IRequest<IReadOnlyList<NormalizedAttractionData>>;