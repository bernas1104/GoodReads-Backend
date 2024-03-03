using System.Linq.Expressions;

using GoodReads.Domain.Common.EntityFramework;

namespace GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework
{
    public interface IRepository<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken);
        Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken);
        Task<TAggregate?> GetByIdAsync(
            AggregateRootId<TIdType> id,
            CancellationToken cancellationToken
        );
        Task<TAggregate?> GetByFilterAsync(
            Expression<Func<TAggregate, bool>> expression,
            CancellationToken cancellationToken
        );
        Task<IEnumerable<TAggregate>> GetPaginatedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken
        );
        Task<int> GetCountAsync(CancellationToken cancellationToken);
    }
}