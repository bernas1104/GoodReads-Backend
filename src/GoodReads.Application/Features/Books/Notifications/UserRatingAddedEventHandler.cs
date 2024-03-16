using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Books.Notifications
{
    public sealed class UserRatingAddedEventHandler :
        INotificationHandler<UserRatingAdded>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<UserRatingAddedEventHandler> _logger;

        public UserRatingAddedEventHandler(
            IRepository<Book, BookId, Guid> repository,
            IPublisher publisher,
            ILogger<UserRatingAddedEventHandler> logger
        )
        {
            _repository = repository;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(
            UserRatingAdded notification,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var book = await _repository.GetByIdAsync(
                    BookId.Create(notification.BookId),
                    cancellationToken
                );

                if (book is null)
                {
                    await PublishBookNotFoundError(notification, cancellationToken);
                    return;
                }

                book.AddRating(
                    RatingId.Create(notification.RatingId),
                    notification.Score
                );

                await _repository.UpdateAsync(book, cancellationToken);

                _logger.LogInformation(
                    "Rating ({RatingId}) created for Book ({BookId})",
                    notification.RatingId,
                    notification.BookId
                );
            }
            catch (Exception ex)
            {
                await ReverseAddedRatingAsync(notification, ex, cancellationToken);
            }
        }

        private async Task PublishBookNotFoundError(
            UserRatingAdded notification,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(
                    "Rating ({RatingId}) was not added because Book ({BookId}) " +
                        "was not found",
                    notification.RatingId,
                    notification.BookId
                );

            await _publisher.Publish(
                BookRatingCreationError.Create(notification),
                cancellationToken
            );
        }

        private async Task ReverseAddedRatingAsync(
            UserRatingAdded notification,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(
                exception,
                "Error while trying to add Rating ({RatingId}) to Book ({BookId}). " +
                    "Error: {ErrorMessage}",
                notification.RatingId,
                notification.BookId,
                exception.Message
            );

            await _publisher.Publish(
                BookRatingUnexpectedError.Create(
                    notification.RatingId,
                    notification.UserId
                ),
                cancellationToken
            );
        }
    }
}