namespace GoodReads.Domain.Common.Interfaces.Events
{
    public interface IHasDomainEvents
    {
        public IReadOnlyList<IDomainEvent> DomainEvents { get; }

        public void ClearDomainEvents();
    }
}