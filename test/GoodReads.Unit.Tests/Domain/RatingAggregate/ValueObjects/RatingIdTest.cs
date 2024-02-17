using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.RatingAggregate.ValueObjects
{
    public class RatingIdTest
    {
        [Fact]
        public void GivenCreate_ShouldCreateRatingIdInstanceWithGivenGuid()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var ratingId = RatingId.Create(id);

            // assert
            ratingId.Should().NotBeNull();
            ratingId.Value.Should().Be(id);
        }

        [Fact]
        public void GivenCreateUnique_ShouldCreateUniqueRatingId()
        {
            // arrange & act
            var ratingId = RatingId.CreateUnique();

            // assert
            ratingId.Should().NotBeNull();
        }

        [Fact]
        public void GivenRatingId_WhenGetEqualityComponents_ShouldReturnRatingIdsProperties()
        {
            // arrange
            var id = Guid.NewGuid();
            var ratingId = RatingId.Create(id);

            // act
            var equalityComponents = ratingId.GetEqualityComponents();

            // assert
            equalityComponents.Count().Should().Be(1);
            equalityComponents.ElementAt(0).Should().Be(id);
        }
    }
}