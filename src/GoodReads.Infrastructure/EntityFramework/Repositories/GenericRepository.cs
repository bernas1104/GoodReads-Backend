using System.Linq.Expressions;

using GoodReads.Domain.Common.EntityFramework;

using GoodReads.Application.Common.Repositories.EntityFramework;

using Microsoft.EntityFrameworkCore;
using GoodReads.Application.Common.Pagination;
using LinqKit;

namespace GoodReads.Infrastructure.EntityFramework.Repositories
{
    public class GenericRepository<TContext, TAggregate, TId, TIdType>
        : IRepository<TAggregate, TId, TIdType>
        where TContext : DbContext
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        private readonly TContext _context;
        protected readonly DbSet<TAggregate> _set;

        public GenericRepository(TContext context)
        {
            _context = context;
            _set = context.Set<TAggregate>();
        }

        public async Task AddAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _set.AddAsync(aggregate, cancellationToken);
            await SaveChanges(cancellationToken);
        }

        public Task<TAggregate?> GetByIdAsync(
            AggregateRootId<TIdType> id,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _set.FirstOrDefaultAsync(
                x => x.Id.Equals(id) && x.DeletedAt == null,
                cancellationToken
            );
        }

        public async Task UpdateAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            _set.Update(aggregate);
            await SaveChanges(cancellationToken);
        }

        public Task<TAggregate?> GetByFilterAsync(
            Expression<Func<TAggregate, bool>> expression,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _set.FirstOrDefaultAsync(
                expression,
                cancellationToken
            );
        }

        public async Task<IEnumerable<TAggregate>> GetPaginatedAsync(
            Expression<Func<TAggregate, bool>>? filter,
            int page = PaginationConstants.DefaultPage,
            int pageSize = PaginationConstants.DefaultPageSize,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var expression = filter ?? GetEmptyFilter();
            expression = expression.And(a => a.DeletedAt == null);

            return await _set.AsNoTracking()
                .Where(expression)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _set.CountAsync(cancellationToken);
        }

        public async Task RemoveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _set.Remove(aggregate);

            await SaveChanges(cancellationToken);
        }

        private Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _context.SaveChangesAsync(cancellationToken);
        }

        private Expression<Func<TAggregate, bool>> GetEmptyFilter() =>
            x => true;
    }
}