using System.Linq.Expressions;

using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.Common.MongoDb;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;
using GoodReads.Infrastructure.Mongo.Utils.Facets;

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
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _context.GetCollection<TAggregate>()
                .InsertOneAsync(aggregate, cancellationToken: cancellationToken);
        }

        public async Task UpdateAsync(
            Expression<Func<TAggregate, bool>> expression,
            TAggregate aggregate,
            CancellationToken cancellationToken = default
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
            CancellationToken cancellationToken = default
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

        public async Task<IEnumerable<TAggregate>> GetPaginatedAsync(
            Expression<Func<TAggregate, bool>>? filter,
            int page = PaginationConstants.DefaultPage,
            int size = PaginationConstants.DefaultPageSize,
            CancellationToken cancellationToken = default
        )
        {
            var dataFacet = GetDataFacet(page, size);
            var expression = filter ?? Builders<TAggregate>.Filter.Empty;

            var aggregate = await _context.GetCollection<TAggregate>()
                .Aggregate()
                .Match(expression)
                .Facet(dataFacet)
                .FirstOrDefaultAsync(cancellationToken);

            return aggregate.GetData<TAggregate>();
        }

        private static AggregateFacet<TAggregate, AggregateCountResult> GetCountFacet()
        {
            return AggregateFacet.Create(
                "CountFacet",
                PipelineDefinition<TAggregate, AggregateCountResult>.Create(
                    new List<IPipelineStageDefinition>
                    {
                        PipelineStageDefinitionBuilder.Count<TAggregate>()
                    }
                )
            );
        }

        private static AggregateFacet<TAggregate, TAggregate> GetDataFacet(
            int page,
            int size
        )
        {
            return AggregateFacet.Create(
                "DataFacet",
                PipelineDefinition<TAggregate, TAggregate>.Create(
                    new List<IPipelineStageDefinition>
                    {
                        PipelineStageDefinitionBuilder.Skip<TAggregate>(
                            (page - 1) * size
                        ),
                        PipelineStageDefinitionBuilder.Limit<TAggregate>(size)
                    }
                )
            );
        }
    }
}