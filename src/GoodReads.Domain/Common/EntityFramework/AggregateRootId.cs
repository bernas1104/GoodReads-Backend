namespace GoodReads.Domain.Common.EntityFramework
{
    public abstract class AggregateRootId<TIdType> : ValueObject
    {
        public abstract TIdType Value { get; protected set; }
    }
}