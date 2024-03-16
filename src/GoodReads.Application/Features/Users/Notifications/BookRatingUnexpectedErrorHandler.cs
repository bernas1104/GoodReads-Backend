using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.Enums;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Notifications
{
    public sealed class BookRatingUnexpectedErrorHandler :
        INotificationHandler<BookRatingUnexpectedError>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<BookRatingUnexpectedErrorHandler> _logger;

        public BookRatingUnexpectedErrorHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<BookRatingUnexpectedErrorHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(
            BookRatingUnexpectedError notification,
            CancellationToken cancellationToken
        )
        {
            var user = await _repository.GetByIdAsync(
                UserId.Create(notification.UserId),
                cancellationToken
            );

            user!.RemoveRating(RatingId.Create(notification.RatingId));
            user.AddDomainEvent(
                UserRatingUnexpectedError.Create(
                    notification.RatingId,
                    ModuleUnexpectedError.Book
                )
            );

            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation(
                "User ({UserId}) Rating ({RatingId}) removed due to unexpected " +
                    "error on Books module",
                user.Id.Value,
                notification.RatingId
            );
        }
    }
}