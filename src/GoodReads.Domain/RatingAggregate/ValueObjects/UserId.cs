using GoodReads.Domain.Common;

namespace GoodReads.Domain.RatingAggregate.ValueObjects
{
    public sealed class UserId : ValueObject
    {
        public Guid Value { get; private set; }

        private UserId(Guid value)
        {
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public static UserId Create(Guid value) => new UserId(value);
    }
}