using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Notifications
{
    public sealed class BookRatingCreationErrorHandler :
        INotificationHandler<BookRatingCreationError>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<BookRatingCreationErrorHandler> _logger;

        public BookRatingCreationErrorHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<BookRatingCreationErrorHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(
            BookRatingCreationError notification,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _repository.GetByIdAsync(
                UserId.Create(notification.UserId),
                cancellationToken
            );

            user!.RemoveRating(RatingId.Create(notification.RatingId));

            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation(
                "Rating ({RatingId}) removed from User ({UserId}) because " +
                    "Book ({BookId}) was not found",
                notification.RatingId,
                notification.UserId,
                notification.BookId
            );
        }

    }
}