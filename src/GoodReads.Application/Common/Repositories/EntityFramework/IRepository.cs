using System.Linq.Expressions;

using GoodReads.Application.Common.Pagination;
using GoodReads.Domain.Common.EntityFramework;

namespace GoodReads.Application.Common.Repositories.EntityFramework
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
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        );
        Task<TAggregate?> GetByIdAsync(
            AggregateRootId<TIdType> id,
            CancellationToken cancellationToken = default
        );
        Task<TAggregate?> GetByFilterAsync(
            Expression<Func<TAggregate, bool>> expression,
            CancellationToken cancellationToken = default
        );
        Task<IEnumerable<TAggregate>> GetPaginatedAsync(
            Expression<Func<TAggregate, bool>>? filter,
            int page = PaginationConstants.DefaultPage,
            int pageSize = PaginationConstants.DefaultPageSize,
            CancellationToken cancellationToken = default
        );
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken);
    }
}