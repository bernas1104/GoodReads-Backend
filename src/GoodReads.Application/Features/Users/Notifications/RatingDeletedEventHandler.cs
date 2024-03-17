using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Notifications
{
    public sealed class RatingDeletedEventHandler :
        INotificationHandler<RatingDeleted>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<RatingDeletedEventHandler> _logger;

        public RatingDeletedEventHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<RatingDeletedEventHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(
            RatingDeleted notification,
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
                "Rating ({RatingId}) removed from User ({UserId})",
                notification.RatingId,
                user.Id.Value
            );
        }

    }
}