using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.UserAggregate.Entities
{
    public class UserTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewUser_ShouldCreateUserInstance()
        {
            // arrange & act
            var user = new User(
                name: _faker.Person.FullName,
                email: _faker.Internet.Email()
            );

            // assert
            user.Should().NotBeNull();
        }

        [Fact]
        public void GivenUser_WhenAddRating_ShouldAddRatingToUsersRatingList()
        {
            // arrange
            var user = new User(
                name: _faker.Person.FullName,
                email: _faker.Internet.Email()
            );

            var ratingId = RatingId.Create(Guid.NewGuid());

            // act
            user.AddRating(ratingId);

            // assert
            user.RatingIds.Count.Should().Be(1);
        }
    }
}