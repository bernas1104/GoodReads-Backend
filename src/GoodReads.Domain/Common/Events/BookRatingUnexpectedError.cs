using GoodReads.Domain.Common.Interfaces.Events;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class BookRatingUnexpectedError : IDomainEvent, INotification
    {
        public Guid RatingId { get; init; }
        public Guid UserId { get; init; }

        private BookRatingUnexpectedError(Guid ratingId, Guid userId)
        {
            RatingId = ratingId;
            UserId = userId;
        }

        public static BookRatingUnexpectedError Create(Guid ratingId, Guid userId) =>
            new BookRatingUnexpectedError(ratingId, userId);
    }
}