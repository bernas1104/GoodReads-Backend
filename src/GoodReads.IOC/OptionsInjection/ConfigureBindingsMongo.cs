using GoodReads.Domain.Common.Interfaces.Repositories.MongoDb;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Infrastructure.Mongo.Contexts;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;
using GoodReads.Infrastructure.Mongo.Repositories;
using GoodReads.Infrastructure.Mongo.Utils;
using GoodReads.Infrastructure.Mongo.Utils.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GoodReads.IOC.OptionsInjection
{
    public static class ConfigureBindingsMongo
    {
        public static void RegisterBindings(
            IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<MongoConnectionOptions>(
                configuration.GetSection(MongoConnectionOptions.Mongo)
            );

            services.AddSingleton<IMongoConnection, MongoConnection>();
            services.AddSingleton<IMongoContext, MongoContext>();

            ConfigureMongoRepositories(services);

            ConfigureMongoSerializer();
        }

        private static void ConfigureMongoRepositories(IServiceCollection services)
        {
            services.AddScoped<
                IRepository<Rating, RatingId, Guid>,
                GenericRepository<Rating, RatingId, Guid>
            >();
        }

        private static void ConfigureMongoSerializer()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            #pragma warning disable CS0618, CS8602
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;

            var objectSerializer = new ObjectSerializer(
                type => ObjectSerializer.DefaultAllowedTypes(type) ||
                    type.FullName.StartsWith("GoodReads.Domain")
            );
            #pragma warning restore

            BsonSerializer.RegisterSerializer(objectSerializer);
        }
    }
}