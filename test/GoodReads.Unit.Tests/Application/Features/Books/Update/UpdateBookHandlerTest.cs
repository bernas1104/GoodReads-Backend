using ErrorOr;

using GoodReads.Application.Features.Books.Update;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Books.Update
{
    public class UpdateBookHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<UpdateBookHandler> _logger;
        private readonly UpdateBookHandler _handler;

        public UpdateBookHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _logger = Substitute.For<ILogger<UpdateBookHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenUpdateBookRequest_WhenBookNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var bookId = Guid.NewGuid();
            var request = BookMock.GetUpdateBookRequest(bookId);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.FirstError.Code.Should().Be("Book.NotFound");

            _logger.ShouldHaveLoggedError($"Book ({bookId}) was not found");

            await _repository.DidNotReceive()
                .UpdateAsync(
                    Arg.Any<Book>(),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GivenUpdateBookRequest_WhenBookFound_ShouldReturnUpdated()
        {
            // arrange
            var book = BookMock.Get();
            var bookId = book.Id.Value;
            var request = BookMock.GetUpdateBookRequest(bookId);

            _repository.GetByIdAsync(
                Arg.Is<BookId>(b => b.Equals(BookId.Create(bookId))),
                Arg.Any<CancellationToken>()
            ).Returns(book);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().Be(Result.Updated);

            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<Book>(b => b.Id.Equals(book.Id)),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation($"Book ({bookId}) was updated successfully");
        }
    }
}