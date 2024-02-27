using System.Linq.Expressions;

using GoodReads.Domain.Common.Interfaces.Repositories.MongoDb;
using GoodReads.Domain.Common.MongoDb;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Repositories
{
    public class GenericRepository<TAggregate, TId, TIdType> :
        IRepository<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        private readonly IMongoContext _context;

        public GenericRepository(IMongoContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            TAggregate aggregate,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _context.GetCollection<TAggregate>()
                .InsertOneAsync(aggregate, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(
            Expression<Func<TAggregate, bool>> expression,
            TAggregate aggregate,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _context.GetCollection<TAggregate>()
                .ReplaceOneAsync(
                    expression,
                    aggregate,
                    cancellationToken: cancellationToken
                );
        }

        public async Task<TAggregate?> GetByIdAsync(
            AggregateRootId<TIdType> id,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = await _context.GetCollection<TAggregate>()
                .FindAsync(
                    x => x.Id.Equals(id),
                    cancellationToken: cancellationToken
                );

            return await queryResult.FirstOrDefaultAsync(cancellationToken);
        }
    }
}