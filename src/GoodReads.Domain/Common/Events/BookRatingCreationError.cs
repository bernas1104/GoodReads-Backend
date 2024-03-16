using GoodReads.Domain.Common.Interfaces.Events;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class BookRatingCreationError : IDomainEvent, INotification
    {
        public Guid RatingId { get; init; }
        public Guid BookId { get; init; }
        public Guid UserId { get; init; }

        private BookRatingCreationError(
            Guid ratingId,
            Guid bookId,
            Guid userId
        )
        {
            RatingId = ratingId;
            BookId = bookId;
            UserId = userId;
        }

        public static BookRatingCreationError Create(UserRatingAdded @event) =>
            new BookRatingCreationError(
                @event.RatingId,
                @event.BookId,
                @event.UserId
            );
    }
}