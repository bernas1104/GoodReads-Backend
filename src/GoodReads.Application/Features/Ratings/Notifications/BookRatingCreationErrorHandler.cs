using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.Notifications
{
    public sealed class BookRatingCreationErrorHandler :
        INotificationHandler<BookRatingCreationError>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<BookRatingCreationErrorHandler> _logger;

        public BookRatingCreationErrorHandler(
            IRepository<Rating, RatingId, Guid> repository,
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

            await _repository.DeleteAsync(
                RatingId.Create(notification.RatingId),
                cancellationToken
            );

            _logger.LogInformation(
                "Rating ({RatingId}) removed because Book ({BookId}) was not found",
                notification.RatingId,
                notification.BookId
            );
        }
    }
}