using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.RatingAggregate.ValueObjects
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

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void GivenNewScore_WhenInvalidValue_ShouldThrowDomainException(
            int value
        )
        {
            // arrange & act
            var func = () => new Score(value);

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'Score' must be between 1 and 5");
        }
    }
}