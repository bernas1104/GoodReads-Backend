using GoodReads.Domain.BookAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.BookAggregate.ValueObject
{
    public class MeanScoreTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenMeanScoreGetValue_WhenNoRatings_ShouldReturnDefault()
        {
            // arrange & act
            var meanScore = new MeanScore();

            // assert
            meanScore.Value.Should().Be(default);
        }

        [Fact]
        public void GivenMeanScore_WhenUpdate_ShouldUpdateMeanScoreValue()
        {
            // arrange
            var mean = 0m;
            var totalRatings = _faker.Random.Int(5, 10);
            var meanScore = new MeanScore();

            // act
            for(var i = 0; i < totalRatings; i++)
            {
                var score = _faker.Random.Int(1, 5);

                mean += score;

                meanScore.Update(score);
            }

            mean /= totalRatings;

            // assert
            meanScore.Value.Should().Be(mean);
        }
    }
}