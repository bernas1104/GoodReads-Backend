using System.Text.Json.Serialization;

using GoodReads.Domain.Common.Interfaces.Events;

#pragma warning disable CS8618
namespace GoodReads.Domain.Common.EntityFramework
{
    public abstract class Entity<TIdType> : BaseEntity, IHasDomainEvents
    {
        public TIdType Id { get; protected set; }
        [JsonIgnore]
        public IReadOnlyList<IDomainEvent> DomainEvents { get => _domainEvents.AsReadOnly(); }
        private readonly List<IDomainEvent> _domainEvents = new ();

        public void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}