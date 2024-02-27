using System.Text.Json.Serialization;

using GoodReads.Domain.Common.Interfaces.Events;

namespace GoodReads.Domain.Common
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        [JsonIgnore]
        public IReadOnlyList<IDomainEvent> DomainEvents { get => _domainEvents.AsReadOnly(); }
        private readonly List<IDomainEvent> _domainEvents = new ();

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        protected virtual void Update() => UpdatedAt = DateTime.UtcNow;

        public void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}