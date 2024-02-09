using GoodReads.Domain.Books.ValueObjects;
using GoodReads.Domain.Common.Exceptions;

namespace GoodReads.Unit.Tests.Domain.Books.ValueObjects
{
    public class ReadingTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewReading_WhenValidDates_ShouldCreateReadingInstance()
        {
            // arrange
            var initiatedAt = _faker.Date.Recent();

            // act
            var reading = new Reading(
                initiatedAt: initiatedAt,
                finishedAt: initiatedAt.AddDays(_faker.Random.Int(1, 10))
            );

            // assert
            reading.Should().NotBeNull();
        }

        [Fact]
        public void GivenNewReading_WhenInvalidInitiatedAt_ShouldThrowDomainException()
        {
            // arrange
            var initiatedAt = new DateTime();

            // act
            var func = () => new Reading(
                initiatedAt: initiatedAt,
                finishedAt: initiatedAt.AddDays(_faker.Random.Int(1, 10))
            );

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'InitiatedAt' is required");
        }

        [Fact]
        public void GivenNewReading_WhenInvalidFinishedAt_ShouldThrowDomainException()
        {
            // arrange
            var initiatedAt = _faker.Date.Recent();

            // act
            var func = () => new Reading(
                initiatedAt: initiatedAt,
                finishedAt: initiatedAt.AddDays(_faker.Random.Int(-10, -1))
            );

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'FinishedAt' must be greater than 'InitiatedAt'");
        }
    }
}