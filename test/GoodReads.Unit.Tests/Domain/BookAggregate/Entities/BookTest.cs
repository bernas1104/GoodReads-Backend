using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Shared.Mocks;

namespace GoodReads.Unit.Tests.Domain.BookAggregate.Entities
{
    public class BookTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenBook_WhenAddRating_ShouldAddRatingAndUpdateMeanScore()
        {
            // arrange
            var totalRatings = _faker.Random.Int(5, 10);
            var book = BookMock.Get();
            var meanScoreEqualityCheck = new MeanScore();

            book.MeanScore.Should().Be(meanScoreEqualityCheck);

            // act
            for(var i = 0; i < totalRatings; i++)
            {
                book.AddRating(
                    Guid.NewGuid(),
                    _faker.Random.Int(1, 5)
                );
            }

            // assert
            book.Ratings.Count.Should().Be(totalRatings);
            book.MeanScore.Should().NotBe(meanScoreEqualityCheck);
        }
    }
}