using System.Linq.Expressions;

using GoodReads.Domain.Common.MongoDb;

namespace GoodReads.Domain.Common.Interfaces.Repositories.MongoDb
{
    public interface IRepository<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken);
        Task UpdateAsync(Expression<Func<TAggregate, bool>> expression, TAggregate aggregate, CancellationToken cancellationToken);
        Task<TAggregate?> GetByIdAsync(AggregateRootId<TIdType> id, CancellationToken cancellationToken);
    }
}