using GoodReads.Domain.Common.Interfaces.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GoodReads.Infrastructure.EntityFramework.Interceptors
{
    public class DomainEventsInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _publisher;

        public DomainEventsInterceptor(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result
        )
        {
            PublishDomainEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }

        public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
        )
        {
            await PublishDomainEvents(eventData.Context, cancellationToken);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task PublishDomainEvents(
            DbContext? dbContext,
            CancellationToken cancellationToken = default
        )
        {
            if (dbContext is null)
            {
                return;
            }

            var entitiesWithDomainEvents = dbContext.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithDomainEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entitiesWithDomainEvents.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}