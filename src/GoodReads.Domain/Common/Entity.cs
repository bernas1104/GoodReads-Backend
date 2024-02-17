using GoodReads.Domain.Common.Interfaces.Events;

namespace GoodReads.Domain.Common
{
    public abstract class Entity<TId> : IHasDomainEvents
    {
        public TId Id { get; protected set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public IReadOnlyList<IDomainEvent> DomainEvents { get => _domainEvents.AsReadOnly(); }
        private readonly List<IDomainEvent> _domainEvents = new ();

        #pragma warning disable CS8618
        protected Entity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        #pragma warning restore

        protected virtual void Update() => UpdatedAt = DateTime.UtcNow;

        public void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}