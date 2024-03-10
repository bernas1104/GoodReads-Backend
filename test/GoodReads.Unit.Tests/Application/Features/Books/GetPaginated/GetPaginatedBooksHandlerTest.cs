using GoodReads.Application.Features.Books.GetPaginated;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Shared.Mocks;

namespace GoodReads.Unit.Tests.Application.Features.Books.GetPaginated
{
    public class GetPaginatedBooksHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly GetPaginatedBooksHandler _handler;

        public GetPaginatedBooksHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _handler = new (_repository);
        }

        [Fact]
        public async Task GivenGetPaginatedBooksRequest_ShouldReturnPaginatedBooks()
        {
            // arrange
            var request = BookMock.GetPaginatedBooksRequest();
            var books = BookMock.GetBooks(request.Size);
            var count = new Faker().Random.Int(1, 10);

            _repository.GetPaginatedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>()
            ).Returns(books);

            _repository.GetCountAsync(Arg.Any<CancellationToken>())
                .Returns(count);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.Data.Should().HaveCount(books.Count());
            response.CurrentPage.Should().Be(request.Page);
            response.TotalItens.Should().Be(count);
            response.TotalPages.Should().Be((int)Math.Ceiling(count / (decimal)request.Size));
            response.PageSize.Should().Be(request.Size);
        }
    }
}