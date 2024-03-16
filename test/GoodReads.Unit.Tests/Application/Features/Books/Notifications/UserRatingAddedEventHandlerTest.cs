using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Application.Features.Books.Notifications;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Events;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using MediatR;

using Microsoft.Extensions.Logging;

using NSubstitute.ExceptionExtensions;

namespace GoodReads.Unit.Tests.Application.Features.Books.Notifications
{
    public class UserRatingAddedEventHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<UserRatingAddedEventHandler> _logger;
        private readonly UserRatingAddedEventHandler _handler;

        public UserRatingAddedEventHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _publisher = Substitute.For<IPublisher>();
            _logger = Substitute.For<ILogger<UserRatingAddedEventHandler>>();
            _handler = new (_repository, _publisher, _logger);
        }

        [Fact]
        public async Task GivenUserRatingAddedEvent_WhenBookExists_ShouldAddRatingToBook()
        {
            // arrange
            var book = BookMock.Get();
            var @event = UserMock.GetUserRatingAdded(bookId: book.Id.Value);

            _repository.GetByIdAsync(
                Arg.Any<BookId>(),
                Arg.Any<CancellationToken>()
            ).Returns(book);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<Book>(b => b.Id.Equals(book.Id)),
                    Arg.Any<CancellationToken>()
                );

            await _publisher.DidNotReceive()
                .Publish(
                    Arg.Any<BookRatingCreationError>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({@event.RatingId}) created for Book ({@event.BookId})"
            );
        }

        [Fact]
        public async Task GivenUserRatingAddedEvent_WhenBookDoesNotExist_ShouldPublishErrorEvent()
        {
            // arrange
            var @event = UserMock.GetUserRatingAdded();

            _repository.GetByIdAsync(
                Arg.Any<BookId>(),
                Arg.Any<CancellationToken>()
            ).Returns((Book?)null);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.DidNotReceive()
                .UpdateAsync(
                    Arg.Any<Book>(),
                    Arg.Any<CancellationToken>()
                );

            await _publisher.Received()
                .Publish(
                    Arg.Is<BookRatingCreationError>(
                        e => e.RatingId == @event.RatingId &&
                            e.BookId == @event.BookId &&
                            e.UserId == @event.UserId
                    ),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError(
                $"Rating ({@event.RatingId}) was not added because Book " +
                    $"({@event.BookId}) was not found"
            );
        }

        [Fact]
        public async Task GivenUserRatingAddedEvent_WhenUnexpectedErrorOccurs_ShouldPublishBookRatingUnexpectedError()
        {
            // arrange
            var @event = UserMock.GetUserRatingAdded();
            var exception = new Exception("Some Error");

            _repository.GetByIdAsync(
                Arg.Any<BookId>(),
                Arg.Any<CancellationToken>()
            ).ThrowsAsync(exception);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _publisher.Received()
                .Publish(
                    Arg.Any<BookRatingUnexpectedError>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError(
                $"Error while trying to add Rating ({@event.RatingId}) to Book " +
                    $"({@event.BookId}). Error: {exception.Message}"
            );
        }
    }
}