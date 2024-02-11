using GoodReads.Domain.Common;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Domain.RatingAggregate.Entities
{
    public sealed class Rating : Entity
    {
        public Score Score { get; private set; }
        public string Description { get; private set; }
        public Reading Reading { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BookId { get; private set; }

        public Rating(
            Score score,
            string description,
            Reading reading,
            Guid userId,
            Guid bookId
        )
        {
            Score = score;
            Description = description;
            Reading = reading;
            UserId = userId;
            BookId = bookId;
        }
    }
}