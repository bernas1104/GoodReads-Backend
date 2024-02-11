using GoodReads.Domain.Common.Interfaces.Events;

namespace GoodReads.Domain.Common
{
    public abstract class Entity : IHasDomainEvents
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public IReadOnlyList<IDomainEvent> DomainEvents { get => _domainEvents.AsReadOnly(); }
        private readonly List<IDomainEvent> _domainEvents = new ();

        protected Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        protected virtual void Update() => UpdatedAt = DateTime.UtcNow;

        public void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}