using GoodReads.Domain.Common;
using GoodReads.Domain.RatingAggregate.Events;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Domain.RatingAggregate.Entities
{
    public sealed class Rating : AggregateRoot<RatingId, Guid>
    {
        public Score Score { get; private set; }
        public string Description { get; private set; }
        public Reading Reading { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BookId { get; private set; }

        private Rating(
            Score score,
            string description,
            Reading reading,
            Guid userId,
            Guid bookId
        ) : base(RatingId.CreateUnique())
        {
            Score = score;
            Description = description;
            Reading = reading;
            UserId = userId;
            BookId = bookId;
        }

        public static Rating Create(
            Score score,
            string description,
            Reading reading,
            Guid userId,
            Guid bookId
        )
        {
            var rating = new Rating(
                score,
                description,
                reading,
                userId,
                bookId
            );

            rating.AddDomainEvent(new RatingCreated(rating));

            return rating;
        }
    }
}