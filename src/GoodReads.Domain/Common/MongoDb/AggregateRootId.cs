namespace GoodReads.Domain.Common.MongoDb
{
    public abstract class AggregateRootId<TIdType> : ValueObject
    {
        public abstract TIdType Value { get; protected set; }
    }
}