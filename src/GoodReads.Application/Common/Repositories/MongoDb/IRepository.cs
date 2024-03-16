using System.Linq.Expressions;

using GoodReads.Application.Common.Pagination;
using GoodReads.Domain.Common.MongoDb;

namespace GoodReads.Application.Common.Repositories.MongoDb
{
    public interface IRepository<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        Task AddAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        );
        Task UpdateAsync(
            Expression<Func<TAggregate, bool>> expression,
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        );
        Task<TAggregate?> GetByIdAsync(
            AggregateRootId<TIdType> id,
            CancellationToken cancellationToken = default
        );
        Task<IEnumerable<TAggregate>> GetPaginatedAsync(
            Expression<Func<TAggregate, bool>>? filter,
            int page = PaginationConstants.DefaultPage,
            int size = PaginationConstants.DefaultPageSize,
            CancellationToken cancellationToken = default
        );
        Task<long> GetCountAsync(
            Expression<Func<TAggregate, bool>>? filter = null,
            CancellationToken cancellationToken = default
        );
        Task<TAggregate?> GetByFilterAsync(
            Expression<Func<TAggregate, bool>> filter,
            CancellationToken cancellationToken = default
        );
        Task DeleteAsync(AggregateRootId<TIdType> id, CancellationToken cancellationToken);
    }
}