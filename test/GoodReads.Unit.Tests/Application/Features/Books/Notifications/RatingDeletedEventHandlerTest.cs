using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Application.Features.Books.Notifications;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Events;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Books.Notifications
{
    public class RatingDeletedEventHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<RatingDeletedEventHandler> _logger;
        private readonly RatingDeletedEventHandler _handler;

        public RatingDeletedEventHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _logger = Substitute.For<ILogger<RatingDeletedEventHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenRatingDeletedEvent_ShouldRemoveRatingFromBook()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var rating = RatingMock.Get(
                GoodReads.Domain.RatingAggregate.ValueObjects.RatingId.Create(ratingId),
                Guid.NewGuid(),
                Guid.NewGuid()
            );
            var @event = RatingDeleted.Create(rating);
            var book = BookMock.Get();
            book.AddRating(
                RatingId.Create(ratingId),
                rating.Score.Value
            );

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

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({ratingId}) removed from Book ({book.Id.Value})"
            );
        }
    }
}