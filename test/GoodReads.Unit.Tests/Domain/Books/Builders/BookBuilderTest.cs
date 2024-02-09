using GoodReads.Domain.Books.Builders;
using GoodReads.Domain.Books.Enums;
using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.Common.Interfaces.Providers;
using GoodReads.Shared.Providers;

namespace GoodReads.Unit.Tests.Domain.Books.Builders
{
    public class BookBuilderTest
    {
        private readonly BookBuilder _builder;
        private readonly IDateProvider _dateProvider;
        private readonly Faker _faker = new ();

        public BookBuilderTest()
        {
            _builder = new BookBuilder(
                _faker.Random.String2(6),
                _faker.Random.String2(6),
                _faker.Random.String2(6),
                Gender.Novel
            );

            _dateProvider = new FakeDateProvider(DateTime.UtcNow);
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithCover_ShouldReturnBookWithCover()
        {
            // arrange
            var cover = _faker.Random.Bytes(256);

            // act
            _builder.AddCover(cover);

            var book = _builder.GetBook();

            // assert
            book.Should().NotBeNull();
            book.Cover.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenBookBuilder_WhenBuildWithInvalidDescription_ShouldThrowDomainException(
            string description
        )
        {
            // arrange & act
            var func = () => _builder.AddDescription(description);

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'Description' is required");
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithValidDescription_ShouldReturnBookWithDescription()
        {
            // arrange
            var description = _faker.Random.String2(10);

            // act
            _builder.AddDescription(description);

            var book = _builder.GetBook();

            // assert
            book.Should().NotBeNull();
            book.Description.Should().NotBeNullOrEmpty();
            book.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithInvalidPages_ShouldThrowDomainException()
        {
            // arrange & act
            var func = () => _builder.AddPages(0);

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'Pages' must be greater than 0");
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithValidPages_ShouldReturnBookWithPages()
        {
            // arrange
            var pages = _faker.Random.Int(1, 500);

            // act
            _builder.AddPages(pages);

            var book = _builder.GetBook();

            // assert
            book.Should().NotBeNull();
            book.Pages.Should().BePositive();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenBookBuilder_WhenBuildWithInvalidPublisher_ShouldThrowDomainException(
            string publisher
        )
        {
            // arrange & act
            var func = () => _builder.AddPublisher(publisher);

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage("'Publisher' is required");
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithValidPublisher_ShouldReturnBookWithPublisher()
        {
            // arrange
            var publisher = _faker.Random.String2(10);

            // act
            _builder.AddPublisher(publisher);

            var book = _builder.GetBook();

            // assert
            book.Should().NotBeNull();
            book.Publisher.Should().NotBeNullOrEmpty();
            book.Publisher.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithTooOldYearOfPublication_ShouldThrowDomainException()
        {
            // arrange & act
            var func = () => _builder.AddYearOfPublication(1899, _dateProvider);
            
            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage($"'YearOfPublication' must be between 1900 and {_dateProvider.GetCurrentYear()}");
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithFutureYearOfPublication_ShouldThrowDomainException()
        {
            // arrange
            var nextYear = DateTime.UtcNow.Year + 1;
            
            // act
            var func = () => _builder.AddYearOfPublication(nextYear, _dateProvider);
            
            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage($"'YearOfPublication' must be between 1900 and {_dateProvider.GetCurrentYear()}");
        }

        [Fact]
        public void GivenBookBuilder_WhenBuildWithValidYearOfPublication_ShouldReturnBookWithYearOfPublication()
        {
            // arrange
            var currentYear = DateTime.UtcNow.Year;
            var year = _faker.Random.Int(1900, currentYear);

            // act
            _builder.AddYearOfPublication(year, _dateProvider);

            var book = _builder.GetBook();
            
            // assert
            book.Should().NotBeNull();
            book.YearOfPublication
                .Should()
                .BeGreaterThan(1899);
            book.YearOfPublication
                .Should()
                .BeLessThanOrEqualTo(currentYear);
        }
    }
}