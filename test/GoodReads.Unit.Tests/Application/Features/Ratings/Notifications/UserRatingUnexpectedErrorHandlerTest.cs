using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings.Notifications;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Domain.UserAggregate.Enums;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.Notifications
{
    public class UserRatingUnexpectedErrorHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<UserRatingUnexpectedErrorHandler> _logger;
        private readonly UserRatingUnexpectedErrorHandler _handler;

        public UserRatingUnexpectedErrorHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _logger = Substitute.For<ILogger<UserRatingUnexpectedErrorHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenUserRatingUnexpectedError_ShouldDeleteRatingFromRepository()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var rating = RatingMock.Get(RatingId.Create(ratingId));
            var errorOrigin = ModuleUnexpectedError.FromValue(
                new Faker().Random.Int(0, 1)
            );
            var @event = UserRatingUnexpectedError.Create(
                ratingId,
                errorOrigin
            );

            _repository.GetByIdAsync(
                Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                Arg.Any<CancellationToken>()
            ).Returns(rating);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .DeleteAsync(
                    Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({ratingId}) removed due to unexpected error on " +
                    $"{errorOrigin.Name} module"
            );
        }
    }
}