using GoodReads.Domain.RatingAggregate.Entities;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class RatingDeleted : INotification
    {
        public Guid RatingId { get; init; }
        public Guid UserId { get; init; }
        public Guid BookId { get; init; }
        public int Score { get; init; }

        private RatingDeleted(Guid ratingId, Guid userId, Guid bookId, int score)
        {
            RatingId = ratingId;
            UserId = userId;
            BookId = bookId;
            Score = score;
        }

        public static RatingDeleted Create(Rating rating) =>
            new RatingDeleted(
                rating.Id.Value,
                rating.UserId.Value,
                rating.BookId.Value,
                rating.Score.Value
            );
    }
}