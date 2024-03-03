using System.Linq.Expressions;

using GoodReads.Domain.Common.EntityFramework;

using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;

using Microsoft.EntityFrameworkCore;

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
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _set.AddAsync(aggregate, cancellationToken);
            await SaveChanges(cancellationToken);
        }

        public Task<TAggregate?> GetByIdAsync(
            AggregateRootId<TIdType> id,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _set.FirstOrDefaultAsync(
                x => x.Id.Equals(id), cancellationToken);
        }

        public async Task UpdateAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            _set.Update(aggregate);
            await SaveChanges(cancellationToken);
        }

        public Task<TAggregate?> GetByFilterAsync(
            Expression<Func<TAggregate, bool>> expression,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _set.FirstOrDefaultAsync(
                expression,
                cancellationToken
            );
        }

        public async Task<IEnumerable<TAggregate>> GetPaginatedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _set.AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _set.CountAsync(cancellationToken);
        }


        private Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}