using GoodReads.Domain.Common;

namespace GoodReads.Domain.UserAggregate.ValueObjects
{
    public sealed class RatingId : ValueObject
    {
        public Guid Value { get; private set; }

        private RatingId(Guid value)
        {
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public static RatingId Create(Guid id) => new RatingId(id);
    }
}