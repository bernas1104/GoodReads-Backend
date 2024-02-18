namespace GoodReads.Domain.Common
{
    public abstract class AggregateRoot<TId, TIdType> : Entity<TIdType>
        where TId : EntityId<TIdType>
    {
        protected AggregateRoot(TId id)
        {
            Id = id;
        }
    }
}