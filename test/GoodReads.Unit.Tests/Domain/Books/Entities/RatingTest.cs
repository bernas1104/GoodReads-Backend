using GoodReads.Domain.Books.Entities;
using GoodReads.Domain.Books.ValueObjects;
using GoodReads.Domain.Common.Exceptions;

namespace GoodReads.Unit.Tests.Domain.Books.Entities
{
    public class RatingTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewRating_WhenValidData_ShouldCreateRatingInstace()
        {
            // arrange & act
            var rating = new Rating(
                score: new Score(_faker.Random.Int(1, 5)),
                description: _faker.Random.String2(20),
                userId: Guid.NewGuid(),
                bookId: Guid.NewGuid()
            );

            // assert
            rating.Should().NotBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenNewRating_WhenInvalidDescription_ShouldThrowDomainException(
            string description
        )
        {
            // arrange & act
            var func = () => new Rating(
                score: new Score(_faker.Random.Int(1, 5)),
                description: description,
                userId: Guid.NewGuid(),
                bookId: Guid.NewGuid()
            );

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'Description' is required");
        }

        [Fact]
        public void GivenNewRating_WhenInvalidUserId_ShouldThrowDomainException()
        {
            // arrange & act
            var func = () => new Rating(
                score: new Score(_faker.Random.Int(1, 5)),
                description: _faker.Random.String2(20),
                userId: Guid.Empty,
                bookId: Guid.NewGuid()
            );

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'UserId' is required");
        }

        [Fact]
        public void GivenNewRating_WhenInvalidBookId_ShouldThrowDomainException()
        {
            // arrange & act
            var func = () => new Rating(
                score: new Score(_faker.Random.Int(1, 5)),
                description: _faker.Random.String2(20),
                userId: Guid.NewGuid(),
                bookId: Guid.Empty
            );

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'BookId' is required");
        }
    }
}