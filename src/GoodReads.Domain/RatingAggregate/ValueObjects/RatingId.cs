using System.Text.Json.Serialization;

using GoodReads.Domain.Common.MongoDb;

namespace GoodReads.Domain.RatingAggregate.ValueObjects
{
    public sealed class RatingId : AggregateRootId<Guid>
    {
        public override Guid Value { get; protected set; }

        [JsonConstructor]
        private RatingId(Guid value)
        {
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public static RatingId Create(Guid id) => new RatingId(id);

        public static RatingId CreateUnique() => new RatingId(Guid.NewGuid());
    }
}