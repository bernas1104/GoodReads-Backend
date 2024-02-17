using GoodReads.Domain.Common;

namespace GoodReads.Domain.UserAggregate.ValueObjects
{
    public sealed class UserId : AggregateRootId<Guid>
    {
        public override Guid Value { get; protected set; }

        private UserId(Guid value)
        {
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public static UserId Create(Guid id) => new UserId(id);

        public static UserId CreateUnique() => new UserId(Guid.NewGuid());
    }
}