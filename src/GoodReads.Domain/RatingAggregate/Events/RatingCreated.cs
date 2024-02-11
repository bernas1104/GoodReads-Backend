using GoodReads.Domain.Common.Interfaces.Events;
using GoodReads.Domain.RatingAggregate.Entities;

namespace GoodReads.Domain.RatingAggregate.Events
{
    public sealed record RatingCreated(Rating Rating) : IDomainEvent;
}