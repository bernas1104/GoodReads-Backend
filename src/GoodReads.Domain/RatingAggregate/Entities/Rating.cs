using System.Text.Json.Serialization;

using GoodReads.Domain.Common.MongoDb;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Domain.RatingAggregate.Entities
{
    public sealed class Rating : AggregateRoot<RatingId, Guid>
    {
        public Score Score { get; private set; }
        public string Description { get; private set; }
        public Reading Reading { get; private set; }
        public UserId UserId { get; private set; }
        public BookId BookId { get; private set; }

        [JsonConstructor]
        public Rating(
            RatingId id,
            Score score,
            string description,
            Reading reading,
            UserId userId,
            BookId bookId
        )
        {
            Id = id;
            Score = score;
            Description = description;
            Reading = reading;
            UserId = userId;
            BookId = bookId;
        }

        private Rating(
            RatingId id,
            Score score,
            string description,
            Reading reading,
            Guid userId,
            Guid bookId
        )
        {
            Id = id;
            Score = score;
            Description = description;
            Reading = reading;
            UserId = UserId.Create(userId);
            BookId = BookId.Create(bookId);
        }

        private Rating(
            Score score,
            string description,
            Reading reading,
            Guid userId,
            Guid bookId
        )
        {
            Id = RatingId.CreateUnique();

            Score = score;
            Description = description;
            Reading = reading;
            UserId = UserId.Create(userId);
            BookId = BookId.Create(bookId);
        }

        public void Update(string description)
        {
            Description = description;
            Update();
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

            return rating;
        }

        public static Rating Instantiate(
            RatingId id,
            Score score,
            string description,
            Reading reading,
            Guid userId,
            Guid bookId
        )
        {
            return new Rating(
                id,
                score,
                description,
                reading,
                userId,
                bookId
            );
        }
    }
}