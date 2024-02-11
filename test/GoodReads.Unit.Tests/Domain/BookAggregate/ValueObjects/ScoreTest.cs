using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.BookAggregate.ValueObjects
{
    public class ScoreTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewScore_WhenValidValue_ShouldCreateScoreInstance()
        {
            // arrange & act
            var score = new Score(_faker.Random.Int(1, 5));

            // assert
            score.Should().NotBeNull();
        }
    }
}