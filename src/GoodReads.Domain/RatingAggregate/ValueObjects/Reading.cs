using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;

using Throw;

namespace GoodReads.Domain.RatingAggregate.ValueObjects
{
    public sealed class Reading : ValueObject
    {
        public DateTime InitiatedAt { get; private set; }
        public DateTime FinishedAt { get; private set; }

        public Reading(DateTime initiatedAt, DateTime finishedAt)
        {
            finishedAt.Throw(() => new DomainException("'FinishedAt' must be greater than 'InitiatedAt'"))
                .IfLessThan(initiatedAt);

            InitiatedAt = initiatedAt;
            FinishedAt = finishedAt;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return InitiatedAt;
            yield return FinishedAt;
        }
    }
}