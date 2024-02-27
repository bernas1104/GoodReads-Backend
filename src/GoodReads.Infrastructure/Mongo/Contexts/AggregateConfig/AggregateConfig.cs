using GoodReads.Domain.Common.MongoDb;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Contexts.AggregateConfig
{
    public abstract class AggregateConfig<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        protected IndexKeysDefinitionBuilder<TAggregate> Builder { get; }
        private readonly IMongoContext _context;
        private readonly IMongoCollection<TAggregate>? _collection;
        private readonly List<CreateIndexModel<TAggregate>> _newIndexes;
        private readonly IEnumerable<string?> _existingIndexes = new List<string?>();

        protected AggregateConfig(IMongoContext context)
        {
            _context = context;
            _collection = context.GetCollection<TAggregate>();
            _newIndexes = new ();

            Builder = Builders<TAggregate>.IndexKeys;

            if (_collection is not null)
            {
                _existingIndexes = _collection.Indexes
                    .List()
                    .ToList()
                    .Select(c => c.GetValue("name").ToString());
            }
        }
        protected void AddLogTtlIndex() => AddTtlIndex("Log TTL", _context.LogTtlDays);

        protected void AddTtlIndex(string? indexName = null, int? ttlDays = null)
        {
            indexName ??= "TTL";
            ttlDays ??= _context.LogTtlDays;

            var ttlIndex = new CreateIndexModel<TAggregate>(
                Builder.Ascending(x => x.CreatedAt),
                new CreateIndexOptions
                {
                    Background = true,
                    Name = indexName,
                    ExpireAfter = new TimeSpan((int)ttlDays, 0, 0, 0)
                }
            );

            if (!_existingIndexes.Contains(indexName))
            {
                _newIndexes.Add(ttlIndex);
            }
        }

        public void CreateIndexes()
        {
            ConfigureIndexes();

            if (_collection is not null)
            {
                _collection.Indexes.CreateMany(_newIndexes);
            }
        }

        public abstract void ConfigureIndexes();

        protected void AddIndexes(List<CreateIndexModel<TAggregate>> indexes)
        {
            _newIndexes.AddRange(indexes);
        }
    }
}