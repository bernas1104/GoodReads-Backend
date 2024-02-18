namespace GoodReads.Domain.Common
{
    public abstract class EntityId<TId> : ValueObject
    {
        public abstract TId Value { get; protected set; }
    }
}