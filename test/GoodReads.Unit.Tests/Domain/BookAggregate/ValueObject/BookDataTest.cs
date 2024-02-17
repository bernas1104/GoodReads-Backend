using GoodReads.Domain.BookAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.BookAggregate.ValueObject
{
    public class BookDataTest
    {
        private readonly Faker _faker = new ();

        [Fact]
        public void GivenBookData_WhenGetEqualityComponents_ShouldReturnBookDatasProperties()
        {
            // arrange
            var publisher = _faker.Company.CompanyName();
            var yearOfPublication = _faker.Date.Recent().Year;
            var pages = _faker.Random.Int(100, 500);

            var bookData = new BookData(
                publisher,
                yearOfPublication,
                pages
            );

            // act
            var equalityComponents = bookData.GetEqualityComponents();

            // assert
            equalityComponents.Count().Should().Be(3);
            equalityComponents.ElementAt(0).Should().Be(publisher);
            equalityComponents.ElementAt(1).Should().Be(yearOfPublication);
            equalityComponents.ElementAt(2).Should().Be(pages);
        }
    }
}