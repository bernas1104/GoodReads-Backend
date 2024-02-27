#pragma warning disable CS8618
namespace GoodReads.Domain.Common.MongoDb
{
    public abstract class AggregateRoot<TId, TIdType> : Entity
        where TId : AggregateRootId<TIdType>
    {
        public TId Id { get; protected set; }
    }
}