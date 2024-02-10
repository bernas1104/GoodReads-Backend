using GoodReads.Domain.BookAggregate.Builders;
using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.Common.Interfaces.Providers;
using GoodReads.Shared.Providers;

namespace GoodReads.Unit.Tests.Domain.BookAggregate.Builders
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
        public void GivenBookBuilder_WhenBuildWithValidBookData_ShouldReturnBookWithBookData()
        {
            // arrange
            var publisher = _faker.Company.CompanyName();
            var yearOfPublication = _faker.Date.Recent().Year;
            var pages = _faker.Random.Int(1, 500);

            // act
            _builder.AddBookData(
                publisher,
                yearOfPublication,
                pages,
                _dateProvider
            );

            var book = _builder.GetBook();

            // assert
            book.Should().NotBeNull();
            book.BookData.Should().NotBeNull();
        }

        [Theory]
        [InlineData("", 1950, 300, "'Publisher' is required")]
        [InlineData(" ", 2012, 250, "'Publisher' is required")]
        [InlineData("Publisher", (int)default, 400, "'YearOfPublication' is required")]
        [InlineData("Publisher", null, 400, "'YearOfPublication' must be equal to or less than current year")]
        [InlineData("Publisher", 2004, 0, "'Pages' must be greater than 0")]
        public void GivenBookBuilder_WhenBuildWithInvalidBookData_ShouldThrowDomainException(
            string publisher,
            int? yearOfPublication,
            int pages,
            string message
        )
        {
            // arrange & act
            var func = () => _builder.AddBookData(
                publisher: publisher,
                yearOfPublication: yearOfPublication ?? DateTime.UtcNow.Year + 1,
                pages: pages,
                dateProvider: _dateProvider
            );

            // assert
            func.Should()
                .Throw<DomainException>()
                .WithMessage(message);
        }
    }
}