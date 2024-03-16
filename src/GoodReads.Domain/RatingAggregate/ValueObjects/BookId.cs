using GoodReads.Domain.Common;

namespace GoodReads.Domain.RatingAggregate.ValueObjects
{
    public sealed class BookId : ValueObject
    {
        public Guid Value { get; private set; }

        private BookId(Guid value)
        {
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public static BookId Create(Guid value) => new BookId(value);
    }
}