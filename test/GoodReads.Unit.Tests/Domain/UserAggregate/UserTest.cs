using GoodReads.Domain.UserAggregate.Entities;

namespace GoodReads.Unit.Tests.Domain.UserAggregate
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
    }
}