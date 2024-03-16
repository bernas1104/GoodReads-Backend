using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Application.Features.Users.Notifications;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using MediatR;

using Microsoft.Extensions.Logging;

using NSubstitute.ExceptionExtensions;

namespace GoodReads.Unit.Tests.Application.Features.Users.Notifications
{
    public class RatingCreatedEventHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<RatingCreatedEventHandler> _logger;
        private readonly RatingCreatedEventHandler _handler;

        public RatingCreatedEventHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _publisher = Substitute.For<IPublisher>();
            _logger = Substitute.For<ILogger<RatingCreatedEventHandler>>();
            _handler = new (_repository, _publisher, _logger);
        }

        [Fact]
        public async Task GivenRatingCreatedEvent_WhenUserExists_ShouldAddRatingToUser()
        {
            // arrange
            var user = UserMock.Get();
            var @event = RatingMock.GetRatingCreatedEvent(userId: user.Id.Value);

            _repository.GetByIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<User>(u => u.Id.Equals(user.Id)),
                    Arg.Any<CancellationToken>()
                );

            await _publisher.DidNotReceive()
                .Publish(
                    Arg.Any<UserRatingCreationError>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({@event.RatingId}) created for User ({@event.UserId})"
            );
        }

        [Fact]
        public async Task GivenRatingCreatedEvent_WhenUserDoesNotExist_ShouldPublishErrorEvent()
        {
            // arrange
            var @event = RatingMock.GetRatingCreatedEvent();

            _repository.GetByIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<CancellationToken>()
            ).Returns((User?)null);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.DidNotReceive()
                .UpdateAsync(
                    Arg.Any<User>(),
                    Arg.Any<CancellationToken>()
                );

            await _publisher.Received()
                .Publish(
                    Arg.Is<UserRatingCreationError>(
                        e => e.RatingId == @event.RatingId &&
                            e.BookId == @event.BookId &&
                            e.UserId == @event.UserId
                    ),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError(
                $"Rating ({@event.RatingId}) was not added because User " +
                    $"({@event.UserId}) was not found"
            );
        }

        [Fact]
        public async Task GivenRatingCreatedEvent_WhenUnexpectedErrorOccurs_ShouldPublishUserRatingUnexpectedError()
        {
            // arrange
            var @event = RatingMock.GetRatingCreatedEvent();
            var exception = new Exception("Some Error");

            _repository.GetByIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<CancellationToken>()
            ).ThrowsAsync(exception);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _publisher.Received()
                .Publish(
                    Arg.Any<UserRatingUnexpectedError>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError(
                $"Error while trying to add Rating ({@event.RatingId}) to User " +
                    $"({@event.UserId}). Error: {exception.Message}"
            );
        }
    }
}