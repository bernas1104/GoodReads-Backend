#pragma warning disable CS8618
namespace GoodReads.Domain.Common.EntityFramework
{
    public abstract class AggregateRoot<TId, TIdType> : Entity<TId>
        where TId : AggregateRootId<TIdType>
    {
        public new AggregateRootId<TIdType> Id { get; protected set; }
    }
}