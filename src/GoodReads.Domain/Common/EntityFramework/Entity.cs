using GoodReads.Domain.Common.Interfaces.Events;

#pragma warning disable CS8618
namespace GoodReads.Domain.Common.EntityFramework
{
    public abstract class Entity<TIdType> : BaseEntity, IHasDomainEvents
    {
        public TIdType Id { get; protected set; }
    }
}