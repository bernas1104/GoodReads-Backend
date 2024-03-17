using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Application.Features.Users.Notifications;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Users.Notifications
{
    public class RatingDeletedEventHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<RatingDeletedEventHandler> _logger;
        private readonly RatingDeletedEventHandler _handler;

        public RatingDeletedEventHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
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
            var user = UserMock.Get();
            user.AddRating(RatingId.Create(ratingId));

            _repository.GetByIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<User>(b => b.Id.Equals(user.Id)),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({ratingId}) removed from User ({user.Id.Value})"
            );
        }
    }
}