using GoodReads.Domain.Common.Interfaces.Events;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class UserRatingAdded : IDomainEvent, INotification
    {
        public Guid RatingId { get; init; }
        public int Score { get; init; }
        public Guid UserId { get; init; }
        public Guid BookId { get; init; }

        private UserRatingAdded(RatingCreated @event)
        {
            RatingId = @event.RatingId;
            Score = @event.Score;
            UserId = @event.UserId;
            BookId = @event.BookId;
        }

        public static UserRatingAdded Create(RatingCreated @event) =>
            new UserRatingAdded(@event);
    }
}