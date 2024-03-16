using GoodReads.Domain.Common.Interfaces.Events;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class UserRatingCreationError : IDomainEvent, INotification
    {
        public Guid RatingId { get; init; }
        public Guid BookId { get; init; }
        public Guid UserId { get; init; }

        private UserRatingCreationError(
            Guid ratingId,
            Guid bookId,
            Guid userId
        )
        {
            RatingId = ratingId;
            BookId = bookId;
            UserId = userId;
        }

        public static UserRatingCreationError Create(RatingCreated @event) =>
            new UserRatingCreationError(
                @event.RatingId,
                @event.BookId,
                @event.UserId
            );
    }
}