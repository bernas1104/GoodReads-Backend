using GoodReads.Infrastructure.Mongo.Utils.Interfaces;

using Microsoft.Extensions.Options;

using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Utils
{
    public class MongoConnection : IMongoConnection
    {
        private static readonly object Lock = new ();
        private readonly string _connectionString;
        private readonly string _defaultDatabaseName;
        private readonly Dictionary<string, IMongoDatabase> Databases = new ();

        public MongoConnection(IOptions<MongoConnectionOptions> options)
        {
            _connectionString = options.Value.ConnectionString!;
            _defaultDatabaseName = options.Value.Schema!;
        }

        public IMongoDatabase GetDatabase()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(
                    _connectionString,
                    "Mongo connection string is null"
                );
            }

            lock(Lock)
            {
                Databases.TryGetValue(_connectionString, out var database);

                if (database is not null)
                {
                    return database;
                }

                var urlBuilder = new MongoUrlBuilder(_connectionString);
                var databaseName = urlBuilder.DatabaseName;

                if (databaseName is null && string.IsNullOrEmpty(_defaultDatabaseName))
                {
                    throw new ArgumentNullException(
                        databaseName,
                        "Mongo default database is null"
                    );
                }

                databaseName = _defaultDatabaseName;

                var client = new MongoClient(urlBuilder.ToMongoUrl());
                database = client.GetDatabase(databaseName);

                Register();

                Databases[_connectionString] = database;

                return database;
            }
        }

        private static void Register()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreElements", conventionPack, type => true);
        }
    }
}