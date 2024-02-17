using GoodReads.Domain.Common;

namespace GoodReads.Unit.Tests.Domain.Common
{
    public class ValueObjectTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenEquals_WhenTwoIdenticalValueObjects_ShouldReturnTrue()
        {
            // arrange
            var value = _faker.Random.Int();
            var firstObj = new FooValue(value);
            var secondObj = new FooValue(value);

            // act
            var equals = firstObj.Equals(secondObj);

            // assert
            equals.Should().BeTrue();
        }

        [Fact]
        public void GivenEquals_WhenTwoDifferentValueObjects_ShouldReturnFalse()
        {
            // arrange
            var firstObj = new FooValue(_faker.Random.Int(1, 5));
            var secondObj = new FooValue(_faker.Random.Int(6, 10));

            // act
            var equals = firstObj.Equals(secondObj);

            // assert
            equals.Should().BeFalse();
        }

        [Fact]
        public void GivenEquals_WhenTwoDifferentTypeValueObjects_ShouldReturnFalse()
        {
            // arrange
            var firstObj = new FooValue(_faker.Random.Int());
            var secondObj = new BarValue(_faker.Random.Int());

            // act
            var equals = firstObj.Equals(secondObj);

            // assert
            equals.Should().BeFalse();
        }

        [Fact]
        public void GivenEquals_WhenComparedIsNull_ShouldReturnFalse()
        {
            // arrange
            var firstObj = new FooValue(_faker.Random.Int());
            object? secondObj = null;

            // act
            var equals = firstObj.Equals(secondObj);

            // assert
            equals.Should().BeFalse();
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(null, 0)]
        public void GivenGetHashCode_ShouldReturnObjectHashCode(
            int? barValue,
            int expectedHashCode
        )
        {
            // arrange
            var fooValue = barValue is not null ?
                new FooValue(barValue.Value) :
                new FooValue();

            // act
            var hashCode = fooValue.GetHashCode();

            // assert
            hashCode.Should().Be(expectedHashCode);
        }
    }

    internal class FooValue : ValueObject
    {
        public int? BarValue { get; private set; }

        public FooValue()
        {
            //
        }

        public FooValue(int barValue)
        {
            BarValue = barValue;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return BarValue;
        }
    }

    internal class BarValue : ValueObject
    {
        public int FooValue { get; private set; }

        public BarValue(int fooValue)
        {
            FooValue = fooValue;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FooValue;
        }
    }
}