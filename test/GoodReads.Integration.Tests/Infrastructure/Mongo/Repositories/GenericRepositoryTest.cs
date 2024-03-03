using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Infrastructure.Mongo.Contexts;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;
using GoodReads.Infrastructure.Mongo.Repositories;
using GoodReads.Infrastructure.Mongo.Utils;
using GoodReads.Shared.Mocks;

using Microsoft.Extensions.Options;

using Testcontainers.MongoDb;

namespace GoodReads.Integration.Tests.Infrastructure.Mongo
{
    public class MongoGenericRepositoryTest : IAsyncLifetime
    {
        private readonly MongoDbContainer _mongo = new MongoDbBuilder()
            .Build();
        private IMongoContext? _context;

        [Fact]
        public async Task GivenEntityRepository_WhenAddAsync_ShouldAddEntityToDatabase()
        {
            // arrange
            var rating = RatingMock.Get();
            var repository = GetRepository();

            // act
            await repository.AddAsync(rating, default);

            var persistedRating = await repository.GetByIdAsync(
                rating.Id,
                default
            );

            // assert
            persistedRating.Should().NotBeNull();
            rating.Id.Should().Be(persistedRating!.Id);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenUpdateEntity_ShouldUpdateEntityDocument()
        {
            // arrange
            var ratingId = RatingId.CreateUnique();
            var rating = RatingMock.Get(ratingId);
            var repository = GetRepository();

            await repository.AddAsync(rating, default);

            var updatedRating = RatingMock.Get(ratingId);

            // act
            await repository.UpdateAsync(
                r => r.Id.Equals(ratingId),
                updatedRating,
                default
            );

            updatedRating = await repository.GetByIdAsync(ratingId, default);

            // assert
            rating.Should().NotBeNull();
            rating.Id.Should().Be(updatedRating!.Id);
            rating.Description.Should().NotBe(updatedRating.Description);
        }

        private GenericRepository<Rating, RatingId, Guid> GetRepository()
        {
            var connectionString = _mongo.GetConnectionString();

            var options = Options.Create(
                new MongoConnectionOptions
                {
                    ConnectionString = connectionString,
                    Schema = "GoodReadsTest",
                    LogTtlDays = 1
                }
            );

            var connection = new MongoConnection(options);

            _context = new MongoContext(connection, options);

            return new GenericRepository<Rating, RatingId, Guid>(_context);
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