using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Infrastructure.Mongo.Contexts;
using GoodReads.Shared.Mocks;

using MongoDB.Driver;

namespace GoodReads.Unit.Tests.Infrastructure.Mongo.Contexts
{
    public class MongoContextTest
    {
        [Fact]
        public void GivenMongoConnectionAndOptions_WhenNewMongoContext_ShouldConfigureIndexes()
        {
            // arrange
            var options = MongoMock.GetMongoOptions();
            var connection = MongoMock.GetMongoConnection();

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
            var options = MongoMock.GetMongoOptions();
            var connection = MongoMock.GetMongoConnection();

            var context = new MongoContext(connection, options);

            // act
            var collection = context.GetCollection<Rating>();

            // assert
            collection.Should().NotBeNull();
        }
    }
}