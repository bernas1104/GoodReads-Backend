using GoodReads.Domain.Common.Providers;

namespace GoodReads.Unit.Tests.Domain.Common.Providers
{
    public class DateProviderTest
    {
        private readonly DateProvider _dateProvider;

        public DateProviderTest()
        {
            _dateProvider = new DateProvider();
        }

        [Fact]
        public void GivenDateProvider_WhenGetCurrentYear_ShouldReturnCurrentYear()
        {
            // arrange
            var currentYear = DateTime.UtcNow.Year;

            // act
            var response = _dateProvider.GetCurrentYear();

            // assert
            response.Should().Be(currentYear);
        }
    }
}