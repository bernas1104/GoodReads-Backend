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
    public class BookRatingCreationErrorHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<BookRatingCreationErrorHandler> _logger;
        private readonly BookRatingCreationErrorHandler _handler;

        public BookRatingCreationErrorHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _logger = Substitute.For<ILogger<BookRatingCreationErrorHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenBookRatingCreatingError_ShouldDeleteRating()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = UserMock.Get(UserId.Create(userId));
            var rating = RatingMock.Get(
                GoodReads.Domain.RatingAggregate.ValueObjects.RatingId.Create(ratingId),
                userId,
                bookId
            );
            var @event = BookRatingCreationError.Create(
                UserRatingAdded.Create(
                    RatingCreated.Create(rating)
                )
            );

            _repository.GetByIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            // act
            await _handler.Handle(@event, CancellationToken.None);

            // assert
            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<User>(u => u.Id.Equals(UserId.Create(userId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({@event.RatingId}) removed from User ({@event.UserId}) because " +
                    $"Book ({@event.BookId}) was not found"
            );
        }
    }
}