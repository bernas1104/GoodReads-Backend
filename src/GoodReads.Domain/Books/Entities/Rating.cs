using GoodReads.Domain.Books.ValueObjects;
using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;

using Throw;

namespace GoodReads.Domain.Books.Entities
{
    public sealed class Rating : Entity
    {
        public Score Score { get; private set; }
        public string Description { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BookId { get; private set; }

        public Rating(
            Score score,
            string description,
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
            UserId = userId;
            BookId = bookId;
        }

        private DomainException GeneratePropertyException(string propertyName) =>
            new DomainException($"'{propertyName}' is required");
    }
}