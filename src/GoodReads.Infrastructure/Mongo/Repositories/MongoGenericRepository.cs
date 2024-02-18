using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Interfaces.Repositories;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Repositories
{
    public class MongoGenericRepository<TEntity, TId, TIdType> :
        IRepository<TEntity, TId, TIdType>
        where TEntity : Entity<TIdType>
        where TId : EntityId<TIdType>
    {
        private readonly IMongoContext _context;

        public MongoGenericRepository(IMongoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            TEntity aggregate,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _context.GetCollection<TEntity>()
                .InsertOneAsync(aggregate, cancellationToken: cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(
            TId id,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await _context.GetCollection<TEntity>()
                .FindAsync(
                    x => x.Id.Equals(id),
                    cancellationToken: cancellationToken
                );

            return await queryResult.FirstOrDefaultAsync(cancellationToken);
        }
    }
}