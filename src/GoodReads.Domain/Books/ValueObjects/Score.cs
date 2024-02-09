using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;

using Throw;

namespace GoodReads.Domain.Books.ValueObjects
{
    public sealed class Score : ValueObject
    {
        public decimal Value { get; private set; }

        public Score(decimal value)
        {
            value.Throw(() => new DomainException("'Score' must be an integer between 1 and 5"))
                .IfLessThan(1)
                .IfGreaterThan(5);

            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}