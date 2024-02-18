using System.Linq.Expressions;

using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Interfaces.Repositories;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Repositories
{
    public class MongoGenericRepository<TEntity, TIdType> :
        IRepository<TEntity, TIdType>
        where TEntity : Entity<TIdType>
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

        public async Task UpdateAsync(
            Expression<Func<TEntity, bool>> expression,
            TEntity entity,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _context.GetCollection<TEntity>()
                .ReplaceOneAsync(
                    expression,
                    entity,
                    cancellationToken: cancellationToken
                );
        }

        public async Task<TEntity?> GetByIdAsync(
            EntityId<TIdType> id,
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