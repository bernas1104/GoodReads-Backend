using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.Notifications
{
    public sealed class UserRatingCreationErrorHandler :
        INotificationHandler<UserRatingCreationError>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<UserRatingCreationErrorHandler> _logger;

        public UserRatingCreationErrorHandler(
            IRepository<Rating, RatingId, Guid> repository,
            ILogger<UserRatingCreationErrorHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(
            UserRatingCreationError notification,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _repository.DeleteAsync(
                RatingId.Create(notification.RatingId),
                cancellationToken
            );

            _logger.LogInformation(
                "Rating ({RatingId}) removed because User ({UserId}) was not found",
                notification.RatingId,
                notification.UserId
            );
        }
    }
}