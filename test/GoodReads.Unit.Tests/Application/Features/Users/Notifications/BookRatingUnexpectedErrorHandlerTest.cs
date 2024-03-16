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
    public class BookRatingUnexpectedErrorHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<BookRatingUnexpectedErrorHandler> _logger;
        private readonly BookRatingUnexpectedErrorHandler _handler;

        public BookRatingUnexpectedErrorHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _logger = Substitute.For<ILogger<BookRatingUnexpectedErrorHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenBookRatingUnexpectedError_ShouldRemoveRatingFromUser()
        {
            // arrange
            var userId = Guid.NewGuid();
            var ratingId = Guid.NewGuid();
            var @event = BookRatingUnexpectedError.Create(ratingId, userId);

            _repository.GetByIdAsync(
                Arg.Is<UserId>(u => u.Equals(UserId.Create(userId))),
                Arg.Any<CancellationToken>()
            ).Returns(UserMock.Get(UserId.Create(userId)));

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<User>(u => u.Id.Equals(UserId.Create(userId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"User ({userId}) Rating ({ratingId}) removed due to unexpected " +
                    "error on Books module"
            );
        }
    }
}