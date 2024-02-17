using GoodReads.Domain.UserAggregate.Entities;

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
            var ratingId = Guid.NewGuid();
            var user = new User(
                name: _faker.Person.FullName,
                email: _faker.Internet.Email()
            );

            // act
            user.AddRating(ratingId);

            // assert
            user.Ratings.Count.Should().Be(1);
        }
    }
}