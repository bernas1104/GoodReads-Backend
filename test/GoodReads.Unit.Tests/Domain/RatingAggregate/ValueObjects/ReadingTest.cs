using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.RatingAggregate.ValueObjects
{
    public class ReadingTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewReading_WhenValidFinishedAtDate_ShouldCreateReadingInstance()
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

        [Fact]
        public void GivenReading_WhenGetEqualityComponents_ShouldReturnReadingsProperties()
        {
            // arrange
            var initiatedAt = _faker.Date.Recent();
            var finishedAt = initiatedAt.AddDays(1);
            var reading = new Reading(
                initiatedAt,
                finishedAt
            );

            // act
            var equalityComponents = reading.GetEqualityComponents();

            // assert
            equalityComponents.Count().Should().Be(2);
            equalityComponents.ElementAt(0).Should().Be(initiatedAt);
            equalityComponents.ElementAt(1).Should().Be(finishedAt);
        }
    }
}