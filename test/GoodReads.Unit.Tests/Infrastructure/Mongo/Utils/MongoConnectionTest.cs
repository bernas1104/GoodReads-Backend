using GoodReads.Infrastructure.Mongo.Utils;

using Microsoft.Extensions.Options;

namespace GoodReads.Unit.Tests.Infrastructure.Mongo.Utils
{
    public class MongoConnectionTest
    {
        [Fact]
        public void GivenNoConnectionString_WhenGetDatabase_ShouldThrowArgumentNullException()
        {
            // arrange
            var options = Options.Create(
                new MongoConnectionOptions
                {
                    ConnectionString = string.Empty,
                    Schema = "GoodReadsTest",
                    LogTtlDays = 1
                }
            );

            var connection = new MongoConnection(options);

            // act
            var func = () => connection.GetDatabase();

            // assert
            func.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("Mongo connection string is null");
        }

        [Fact]
        public void GivenNoSchema_WhenGetDatabase_ShouldThrowArgumentNullException()
        {
            // arrange
            var options = Options.Create(
                new MongoConnectionOptions
                {
                    ConnectionString = "mongodb://localhost:27017/",
                    Schema = string.Empty,
                    LogTtlDays = 1
                }
            );

            var connection = new MongoConnection(options);

            // act
            var func = () => connection.GetDatabase();

            // assert
            func.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("Mongo default database is null");
        }

        [Theory]
        [InlineData("mongodb://localhost:27017/")]
        [InlineData("mongodb://localhost:27017/GoodReadsTest")]
        public void GivenDatabaseInfo_WhenGetDatabase_ShouldReturnConnectedDatabase(
            string connectionString
        )
        {
            // arrange
            var options = Options.Create(
                new MongoConnectionOptions
                {
                    ConnectionString = connectionString,
                    Schema = "GoodReadsTest",
                    LogTtlDays = 1
                }
            );

            var connection = new MongoConnection(options);

            // act
            var database = connection.GetDatabase();

            // assert
            database.Should().NotBeNull();
        }
    }
}