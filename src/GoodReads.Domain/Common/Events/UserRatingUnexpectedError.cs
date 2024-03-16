using GoodReads.Domain.Common.Interfaces.Events;
using GoodReads.Domain.UserAggregate.Enums;

using MediatR;

namespace GoodReads.Domain.Common.Events
{
    public sealed class UserRatingUnexpectedError : IDomainEvent, INotification
    {
        public Guid RatingId { get; init; }
        public string ErrorOrigin { get; init; }

        private UserRatingUnexpectedError(
            Guid ratingId,
            ModuleUnexpectedError module
        )
        {
            RatingId = ratingId;
            ErrorOrigin = module.Name;
        }

        public static UserRatingUnexpectedError Create(
            Guid ratingId,
            ModuleUnexpectedError module
        ) => new UserRatingUnexpectedError(ratingId, module);
    }
}