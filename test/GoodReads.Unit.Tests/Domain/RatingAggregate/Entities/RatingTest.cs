using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Shared.Mocks;

namespace GoodReads.Unit.Tests.Domain.RatingAggregate.Entities
{
    public class RatingTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewRating_ShouldCreateRatingInstace()
        {
            // arrange & act
            var rating = Rating.Create(
                score: ScoreMock.Get(),
                description: _faker.Random.String2(20),
                reading: ReadingMock.Get(),
                userId: Guid.NewGuid(),
                bookId: Guid.NewGuid()
            );

            // assert
            rating.Should().NotBeNull();
        }
    }
}