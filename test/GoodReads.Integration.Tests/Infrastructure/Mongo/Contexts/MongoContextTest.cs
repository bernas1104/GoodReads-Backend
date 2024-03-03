using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Infrastructure.Mongo.Contexts;
using GoodReads.Shared.Mocks;

using MongoDB.Driver;

using Testcontainers.MongoDb;

namespace GoodReads.Integration.Tests.Infrastructure.Mongo.Contexts
{
    public class MongoContextTest : IAsyncLifetime
    {
        private readonly MongoDbContainer _mongo = new MongoDbBuilder()
            .Build();

        [Fact]
        public void GivenMongoConnectionAndOptions_WhenNewMongoContext_ShouldConfigureIndexes()
        {
            // arrange
            var options = MongoMock.GetMongoOptions(_mongo.GetConnectionString());
            var connection = MongoMock.GetMongoConnection(options);

            // act
            var context = new MongoContext(connection, options);

            // assert
            context.Should().NotBeNull();

            var indexes = context.GetDatabase()
                .GetCollection<Rating>("Rating")
                .Indexes;

            indexes.List().ToList().Count.Should().NotBe(0);
        }

        [Fact]
        public void GivenMongoContxt_WhenGetCollectionWithNoName_ShouldReturnCollectionByType()
        {
            // arrange
            var options = MongoMock.GetMongoOptions(_mongo.GetConnectionString());
            var connection = MongoMock.GetMongoConnection(options);

            var context = new MongoContext(connection, options);

            // act
            var collection = context.GetCollection<Rating>();

            // assert
            collection.Should().NotBeNull();
        }

        public async Task InitializeAsync()
        {
            await _mongo.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _mongo.DisposeAsync().AsTask();
        }
    }
}