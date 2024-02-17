namespace GoodReads.Domain.Common
{
    public abstract class AggregateRoot<TId, TIdType> : Entity<TId>
        where TId : AggregateRootId<TIdType>
    {
        #pragma warning disable CA1061
        public new AggregateRootId<TIdType> Id { get; protected set; }
        #pragma warning restore

        protected AggregateRoot(TId id)
        {
            Id = id;
        }
    }
}