using GoodReads.Domain.Common.Exceptions;

namespace GoodReads.Unit.Tests.Domain.Common.Exceptions
{
    public class DomainExceptionTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenNewDomainException_WhenMessage_ShouldCreateDomainExceptionInstance()
        {
            // arrange & act
            var exception = new DomainException(_faker.Random.String2(20));

            // assert
            exception.Should().NotBeNull();
        }

        [Fact]
        public void GivenNewDomainException_WhenMessageAndInnerException_ShouldCreateDomainExceptionInstance()
        {
            // arrange & act
            var exception = new DomainException(
                _faker.Random.String2(20),
                new Exception(_faker.Random.String2(20))
            );

            // assert
            exception.Should().NotBeNull();
        }
    }
}