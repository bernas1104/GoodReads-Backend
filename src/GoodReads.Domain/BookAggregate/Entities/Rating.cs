using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;

using Throw;

namespace GoodReads.Domain.BookAggregate.Entities
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
            description.Throw(() => GeneratePropertyException(nameof(description)))
                .IfEmpty()
                .IfWhiteSpace();

            userId.Throw(() => GeneratePropertyException(nameof(userId)))
                .IfDefault();

            bookId.Throw(() => GeneratePropertyException(nameof(bookId)))
                .IfDefault();

            Score = score;
            Description = description;
            Reading = reading;
            UserId = userId;
            BookId = bookId;
        }

        private DomainException GeneratePropertyException(string propertyName) =>
            new DomainException($"'{propertyName}' is required");
    }
}