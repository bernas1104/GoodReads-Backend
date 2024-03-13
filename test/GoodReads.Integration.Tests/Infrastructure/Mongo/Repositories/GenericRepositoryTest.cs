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

        [Fact]
        public async Task GivenEntityRepository_WhenGetPaginated_ShouldReturnEntitiesPage()
        {
            // arrange
            var repository = GetRepository();
            var ratings = new List<Rating>
            {
                RatingMock.Get(),
                RatingMock.Get(),
                RatingMock.Get()
            };

            foreach (var rating in ratings)
            {
                await repository.AddAsync(rating, CancellationToken.None);
            }

            // act
            var result = await repository.GetPaginatedAsync(
                null,
                page: 3,
                size: 1,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenGetPaginatedWithFilter_ShouldReturnEntitiesPage()
        {
            var ratingId = Guid.NewGuid();
            var repository = GetRepository();
            var ratings = new List<Rating>
            {
                RatingMock.Get(),
                RatingMock.Get(),
                RatingMock.Get(RatingId.Create(ratingId))
            };

            foreach (var rating in ratings)
            {
                await repository.AddAsync(rating, CancellationToken.None);
            }

            // act
            var result = await repository.GetPaginatedAsync(
                r => r.Id.Equals(RatingId.Create(ratingId)),
                page: 1,
                size: 10,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.ElementAt(0).Id.Value.Should().Be(ratingId);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenGetCount_ShouldReturnEntityCount()
        {
            // arrange
            var repository = GetRepository();
            var ratings = new List<Rating>
            {
                RatingMock.Get(),
                RatingMock.Get(),
                RatingMock.Get()
            };

            foreach (var rating in ratings)
            {
                await repository.AddAsync(rating, CancellationToken.None);
            }

            // act
            var result = await repository.GetCountAsync(CancellationToken.None);

            // assert
            result.Should().Be(3);
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