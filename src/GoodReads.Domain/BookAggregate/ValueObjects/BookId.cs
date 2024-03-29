using GoodReads.Domain.Common.EntityFramework;

namespace GoodReads.Domain.BookAggregate.ValueObjects
{
    public sealed class BookId : AggregateRootId<Guid>
    {
        public override Guid Value { get; protected set; }

        private BookId(Guid value)
        {
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public static BookId Create(Guid value) => new BookId(value);

        public static BookId CreateUnique() => new BookId(Guid.NewGuid());
    }
}