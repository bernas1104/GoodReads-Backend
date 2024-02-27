using GoodReads.Domain.Common.EntityFramework;

namespace GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework
{
    public interface IRepository<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken);
        Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken);
        Task<TAggregate?> GetByIdAsync(AggregateRootId<TIdType> id, CancellationToken cancellationToken);
    }
}