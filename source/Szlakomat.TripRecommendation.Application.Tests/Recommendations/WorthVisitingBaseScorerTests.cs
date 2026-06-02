using Szlakomat.TripRecommendation.Application.Recommendations.Scoring;

namespace Szlakomat.TripRecommendation.Application.Tests.Recommendations;

public class WorthVisitingBaseScorerTests
{
    [Fact]
    public void Score_UsesWorthVisitingRankerSignalsInsteadOfRawExternalScoreOnly()
    {
        var input = PlanningTestData.Input(
            candidates:
            [
                PlanningTestData.Candidate("expensive", score: 1.00m, preference: 1.00m, price: 100m, availability: 1.00m),
                PlanningTestData.Candidate("cheap", score: 0.80m, preference: 0.80m, price: 10m, availability: 0.80m)
            ]);

        var scores = new WorthVisitingBaseScorer().Score(input);

        Assert.True(scores["expensive"].WorthScore < 1.00m);
        Assert.True(scores["cheap"].PriceComponent > scores["expensive"].PriceComponent);
    }
}
