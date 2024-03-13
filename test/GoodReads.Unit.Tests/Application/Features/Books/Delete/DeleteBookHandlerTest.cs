using ErrorOr;

using GoodReads.Application.Features.Books.Delete;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Books.Delete
{
    public class DeleteBookHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<DeleteBookHandler> _logger;
        private readonly DeleteBookHandler _handler;

        public DeleteBookHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _logger = Substitute.For<ILogger<DeleteBookHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenDeleteBookRequest_WhenBookNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var bookId = Guid.NewGuid();
            var request = new DeleteBookRequest(bookId);

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

            _logger.ShouldHaveLoggedWarning($"Book ({bookId}) was not found");

            await _repository.DidNotReceive()
                .UpdateAsync(
                    Arg.Any<Book>(),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GivenDeleteBookRequest_WhenBookFound_ShouldReturnDeleted()
        {
            // arrange
            var book = BookMock.Get();
            var bookId = book.Id.Value;
            var request = new DeleteBookRequest(bookId);

            _repository.GetByIdAsync(
                Arg.Any<BookId>(),
                Arg.Any<CancellationToken>()
            ).Returns(book);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().Be(Result.Deleted);

            _logger.ShouldHaveLoggedInformation($"Book ({bookId}) was deleted");

            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<Book>(b => b.Id.Equals(book.Id)),
                    Arg.Any<CancellationToken>()
                );
        }
    }
}