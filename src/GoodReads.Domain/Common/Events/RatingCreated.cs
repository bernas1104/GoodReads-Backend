using GoodReads.Domain.Common.Interfaces.Events;
using GoodReads.Domain.RatingAggregate.Entities;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class RatingCreated : IDomainEvent, INotification
    {
        public Guid RatingId { get; init; }
        public int Score { get; init; }
        public Guid UserId { get; init; }
        public Guid BookId { get; init; }

        private RatingCreated(Rating rating)
        {
            RatingId = rating.Id.Value;
            Score = rating.Score.Value;
            UserId = rating.UserId.Value;
            BookId = rating.BookId.Value;
        }

        public static RatingCreated Create(Rating rating) =>
            new RatingCreated(rating);
    }
}