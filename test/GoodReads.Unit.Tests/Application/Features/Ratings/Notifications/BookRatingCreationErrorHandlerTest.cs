using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings.Notifications;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.Notifications
{
    public class BookRatingCreationErrorHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<BookRatingCreationErrorHandler> _logger;
        private readonly BookRatingCreationErrorHandler _handler;

        public BookRatingCreationErrorHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _logger = Substitute.For<ILogger<BookRatingCreationErrorHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenBookRatingCreatingError_ShouldDeleteRating()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var rating = RatingMock.Get(RatingId.Create(ratingId), bookId: bookId);
            var @event = BookRatingCreationError.Create(
                UserRatingAdded.Create(
                    RatingCreated.Create(rating)
                )
            );

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .DeleteAsync(
                    Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({ratingId}) removed because Book ({bookId}) was not found"
            );
        }
    }
}