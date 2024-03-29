using GoodReads.Domain.Common.EntityFramework;
using GoodReads.Domain.Common.Interfaces.Events;

namespace GoodReads.Unit.Tests.Domain.Common
{
    public class EntityTest
    {
        [Fact]
        public void GivenDomainEvent_WhenAddDomainEvent_ShouldIncrementDomainEventsListCount()
        {
            // arrange
            var entity = new FooEntity();
            var domainEvent = new FooEntityEvent();

            var eventsCount = entity.DomainEvents.Count;

            // act
            entity.AddDomainEvent(domainEvent);

            // assert
            entity.DomainEvents.Count.Should().BeGreaterThan(eventsCount);
        }

        [Fact]
        public void GivenEntityWithDomainEventsList_WhenClearDomainEvents_ShouldClearEntityDomainEventList()
        {
            // arrange
            var entity = new FooEntity();
            var domainEvent = new FooEntityEvent();

            var eventsCount = entity.DomainEvents.Count;

            entity.AddDomainEvent(domainEvent);

            // act
            entity.ClearDomainEvents();

            // assert
            entity.DomainEvents.Count.Should().Be(eventsCount);
        }
    }

    internal class FooId : AggregateRootId<Guid>
    {
        public override Guid Value { get; protected set; }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    internal class FooEntity : Entity<Guid>
    {
        //
    }

    internal record FooEntityEvent() : IDomainEvent;
}