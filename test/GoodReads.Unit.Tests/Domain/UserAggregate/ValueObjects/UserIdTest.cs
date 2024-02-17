using GoodReads.Domain.UserAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.UserAggregate.ValueObjects
{
    public class UserIdTest
    {
        [Fact]
        public void GivenCreate_ShouldCreateUserIdInstanceWithGivenGuid()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var userId = UserId.Create(id);

            // assert
            userId.Should().NotBeNull();
            userId.Value.Should().Be(id);
        }

        [Fact]
        public void GivenCreateUnique_ShouldCreateUniqueUserId()
        {
            // arrange & act
            var userId = UserId.CreateUnique();

            // assert
            userId.Should().NotBeNull();
        }

        [Fact]
        public void GivenUserId_WhenGetEqualityComponents_ShouldReturnUserIdsProperties()
        {
            // arrange
            var id = Guid.NewGuid();
            var userId = UserId.Create(id);

            // act
            var equalityComponents = userId.GetEqualityComponents();

            // assert
            equalityComponents.Count().Should().Be(1);
            equalityComponents.ElementAt(0).Should().Be(id);
        }
    }
}