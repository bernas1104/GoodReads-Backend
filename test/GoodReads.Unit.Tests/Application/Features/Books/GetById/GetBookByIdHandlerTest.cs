using GoodReads.Application.Features.Books.GetById;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Books.GetById
{
    public class GetBookByIdHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<GetBookByIdHandler> _logger;
        private readonly GetBookByIdHandler _handler;

        public GetBookByIdHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _logger = Substitute.For<ILogger<GetBookByIdHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenGetBookByIdRequest_WhenBookNotFound_ShouldRetornErrorNotFound()
        {
            // arrange
            var bookId = Guid.NewGuid();
            var request = new GetBookByIdRequest(bookId);

            _repository.GetByIdAsync(
                Arg.Any<BookId>(),
                Arg.Any<CancellationToken>()
            ).Returns((Book?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.FirstError.Code.Should().Be("Book.NotFound");

            _logger.ShouldHaveLoggedError($"Book ({bookId}) was not found");
        }

        [Fact]
        public async Task GivenGetBookByIdRequest_WhenBookFound_ShouldReturnBookResponse()
        {
            // arrange
            var book = BookMock.Get();
            var bookId = book.Id.Value;
            var request = new GetBookByIdRequest(bookId);

            _repository.GetByIdAsync(
                Arg.Is<BookId>(b => b.Equals(BookId.Create(bookId))),
                Arg.Any<CancellationToken>()
            ).Returns(book);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().BeOfType<GetBookByIdResponse>();
        }
    }
}