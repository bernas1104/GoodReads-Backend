using GoodReads.Domain.BookAggregate.ValueObjects;

namespace GoodReads.Unit.Tests.Domain.BookAggregate.ValueObject
{
    public class BookIdTest
    {
        [Fact]
        public void GivenCreate_ShouldCreateBookIdInstanceWithGivenGuid()
        {
            // arrange
            var id = Guid.NewGuid();

            // act
            var bookId = BookId.Create(id);

            // assert
            bookId.Should().NotBeNull();
            bookId.Value.Should().Be(id);
        }

        [Fact]
        public void GivenCreateUnique_ShouldCreateUniqueBookId()
        {
            // arrange & act
            var bookId = BookId.CreateUnique();

            // assert
            bookId.Should().NotBeNull();
        }

        [Fact]
        public void GivenBookId_WhenGetEqualityComponents_ShouldReturnBookIdsProperties()
        {
            // arrange
            var id = Guid.NewGuid();
            var bookId = BookId.Create(id);

            // act
            var equalityComponents = bookId.GetEqualityComponents();

            // assert
            equalityComponents.Count().Should().Be(1);
            equalityComponents.ElementAt(0).Should().Be(id);
        }
    }
}