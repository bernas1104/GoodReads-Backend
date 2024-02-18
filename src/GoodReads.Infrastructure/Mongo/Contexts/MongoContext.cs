using GoodReads.Infrastructure.Mongo.Contexts.EntityConfig;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;
using GoodReads.Infrastructure.Mongo.Utils;
using GoodReads.Infrastructure.Mongo.Utils.Interfaces;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Contexts
{
    public class MongoContext : IMongoContext
    {
        public IMongoConnection _mongoConnection { get; private set; }
        public int LogTtlDays { get; private set; }

        public MongoContext(
            IMongoConnection mongoConnection,
            IOptions<MongoConnectionOptions> options
        )
        {
            _mongoConnection = mongoConnection;

            LogTtlDays = options.Value.LogTtlDays;

            Configure();
        }

        private void Configure()
        {
            new RatingConfig(this).CreateIndexes();
        }

        public IMongoCollection<T> GetCollection<T>(string? name = null)
        {
            var database = _mongoConnection.GetDatabase();

            IMongoCollection<T> collection = (name != null) ?
                database.GetCollection<T>(name) :
                GetCollection<T>(database);

            return collection;
        }

        public static IMongoCollection<T> GetCollection<T>(IMongoDatabase database)
        {
            var attrs = typeof(T)
                .GetCustomAttributes(typeof(CollectionNameAttribute), false)
                .OfType<CollectionNameAttribute>()
                .FirstOrDefault();

            var collectioName = attrs?.Name ?? typeof(T).Name;

            return database.GetCollection<T>(collectioName);
        }

        public IMongoDatabase GetDatabase()
        {
            return _mongoConnection.GetDatabase();
        }

    }
}