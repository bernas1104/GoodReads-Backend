namespace GoodReads.Domain.Common.Interfaces.Repositories
{
    public interface IRepository<TEntity, TId, TIdType>
        where TEntity : Entity<TIdType>
        where TId : EntityId<TIdType>
    {
        Task AddAsync(TEntity aggregate, CancellationToken cancellationToken);
        Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken);
    }
}