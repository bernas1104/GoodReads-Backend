using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.Notifications
{
    public sealed class UserRatingUnexpectedErrorHandler :

        INotificationHandler<UserRatingUnexpectedError>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<UserRatingUnexpectedErrorHandler> _logger;

        public UserRatingUnexpectedErrorHandler(
            IRepository<Rating, RatingId, Guid> repository,
            ILogger<UserRatingUnexpectedErrorHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(
            UserRatingUnexpectedError notification,
            CancellationToken cancellationToken
        )
        {
            await _repository.DeleteAsync(
                RatingId.Create(notification.RatingId),
                cancellationToken
            );

            _logger.LogInformation(
                "Rating ({RatingId}) removed due to unexpected error on " +
                    "{ErrorOrigin} module",
                notification.RatingId,
                notification.ErrorOrigin
            );
        }
    }
}