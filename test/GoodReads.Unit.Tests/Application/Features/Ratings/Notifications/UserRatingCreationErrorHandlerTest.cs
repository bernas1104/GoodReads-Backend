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
    public class UserRatingCreationErrorHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<UserRatingCreationErrorHandler> _logger;
        private readonly UserRatingCreationErrorHandler _handler;

        public UserRatingCreationErrorHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _logger = Substitute.For<ILogger<UserRatingCreationErrorHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenUserRatingCreationError_ShouldDeleteRating()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var rating = RatingMock.Get(RatingId.Create(ratingId), userId);
            var @event = UserRatingCreationError.Create(
                RatingCreated.Create(rating)
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
                $"Rating ({ratingId}) removed because User ({userId}) was not found"
            );
        }
    }
}