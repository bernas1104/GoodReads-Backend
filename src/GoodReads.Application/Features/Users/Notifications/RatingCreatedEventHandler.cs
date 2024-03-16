using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.Enums;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Notifications
{
    public sealed class RatingCreatedEventHandler :
        INotificationHandler<RatingCreated>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<RatingCreatedEventHandler> _logger;

        public RatingCreatedEventHandler(
            IRepository<User, UserId, Guid> repository,
            IPublisher publisher,
            ILogger<RatingCreatedEventHandler> logger
        )
        {
            _repository = repository;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Handle(
            RatingCreated notification,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var user = await _repository.GetByIdAsync(
                    UserId.Create(notification.UserId),
                    cancellationToken
                );

                if (user is null)
                {
                    await PublishUserNotFoundError(notification, cancellationToken);
                    return;
                }

                user.AddRating(RatingId.Create(notification.RatingId));
                user.AddDomainEvent(UserRatingAdded.Create(notification));

                await _repository.UpdateAsync(user, cancellationToken);

                _logger.LogInformation(
                    "Rating ({RatingId}) created for User ({UserId})",
                    notification.RatingId,
                    notification.UserId
                );
            }
            catch (Exception ex)
            {
                await ReverseAddedRatingAsync(notification, ex, cancellationToken);
            }
        }

        private async Task PublishUserNotFoundError(
            RatingCreated notification,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(
                "Rating ({RatingId}) was not added because User ({UserId}) " +
                    "was not found",
                notification.RatingId,
                notification.UserId
            );

            await _publisher.Publish(
                UserRatingCreationError.Create(notification),
                cancellationToken
            );
        }

        private async Task ReverseAddedRatingAsync(
            RatingCreated notification,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(
                exception,
                "Error while trying to add Rating ({RatingId}) to User ({UserId}). " +
                    "Error: {ErrorMessage}",
                notification.RatingId,
                notification.UserId,
                exception.Message
            );

            await _publisher.Publish(
                UserRatingUnexpectedError.Create(
                    notification.RatingId,
                    ModuleUnexpectedError.User
                ),
                cancellationToken
            );
        }
    }
}