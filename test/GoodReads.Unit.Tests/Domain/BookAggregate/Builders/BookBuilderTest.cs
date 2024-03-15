using GoodReads.Domain.BookAggregate.Builders;
using GoodReads.Domain.BookAggregate.Enums;
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

        /*
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
        */

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
                pages
            );

            var book = _builder.GetBook();

            // assert
            book.Should().NotBeNull();
            book.BookData.Should().NotBeNull();
        }
    }
}