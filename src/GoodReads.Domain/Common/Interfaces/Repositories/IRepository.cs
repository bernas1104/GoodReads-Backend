using System.Linq.Expressions;

namespace GoodReads.Domain.Common.Interfaces.Repositories
{
    public interface IRepository<TEntity, TIdType> where TEntity : Entity<TIdType>
    {
        Task AddAsync(TEntity aggregate, CancellationToken cancellationToken);
        Task UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity entity, CancellationToken cancellationToken);
        Task<TEntity?> GetByIdAsync(EntityId<TIdType> id, CancellationToken cancellationToken);
    }
}