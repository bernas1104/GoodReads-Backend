using Bogus;

using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.Common.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Infrastructure.EntityFramework.Contexts;
using GoodReads.Infrastructure.EntityFramework.Repositories;
using GoodReads.Infrastructure.EntityFramework.Utils;
using GoodReads.Shared.Mocks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.MsSql;

namespace GoodReads.Integration.Tests.Infrastructure.EntityFramework.Repositories
{
    public partial class GenericRepositoryTest : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSql = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
            .WithPortBinding(1434, 1433)
            .WithEnvironment("ACCEPT_EULA", "true")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        [Fact]
        public async Task GivenEntityRepository_WhenAddAsync_ShouldAddEntityToDatabase()
        {
            // arrange
            var user = UserMock.Get();
            var repository = GetRepository();

            // act
            await repository.AddAsync(user, default);

            var persistedUser = await repository.GetByIdAsync(
                user.Id,
                default
            );

            // assert
            persistedUser.Should().NotBeNull();
            user.Id.Should().Be(persistedUser!.Id);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenUpdateEntity_ShouldUpdateEntity()
        {
            // arrange
            var userId = UserId.CreateUnique();
            var user = UserMock.Get(userId);
            var oldName = user.Name;
            var repository = GetRepository();

            await repository.AddAsync(user, default);

            var faker = new Faker();
            user.Update(faker.Person.FullName, faker.Internet.Email());

            // act
            await repository.UpdateAsync(user, default);

            var updatedUser = await repository.GetByIdAsync(userId, default);

            // assert
            user.Should().NotBeNull();
            user.Id.Should().Be(updatedUser!.Id);
            updatedUser.Name.Should().NotBe(oldName);
            user.Email.Should().Be(updatedUser.Email);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenGetByFilter_ShouldReturnFirstOrDefault()
        {
            // arrange
            var user = UserMock.Get();
            var userEmail = user.Email;
            var repository = GetRepository();

            await repository.AddAsync(user, CancellationToken.None);

            // act
            var result = await repository.GetByFilterAsync(
                u => u.Email == userEmail,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(userEmail);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenGetPaginated_ShouldReturnEntitiesPage()
        {
            // arrange
            var repository = GetRepository();
            var users = new List<User>
            {
                UserMock.Get(),
                UserMock.Get(),
                UserMock.Get()
            };

            foreach (var user in users)
            {
                await repository.AddAsync(user, CancellationToken.None);
            }

            // act
            var result = await repository.GetPaginatedAsync(
                null,
                page: 3,
                pageSize: 1,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenGetPaginatedWithFilter_ShouldReturnEntitiesPage()
        {
            // arrange
            var userId = Guid.NewGuid();
            var repository = GetRepository();
            var users = new List<User>
            {
                UserMock.Get(),
                UserMock.Get(),
                UserMock.Get(UserId.Create(userId))
            };

            foreach (var user in users)
            {
                await repository.AddAsync(user, CancellationToken.None);
            }

            // act
            var result = await repository.GetPaginatedAsync(
                u => u.Id.Equals(UserId.Create(userId) as AggregateRootId<Guid>),
                page: 1,
                pageSize: 10,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.ElementAt(0).Id.Value.Should().Be(userId);
        }

        [Fact]
        public async Task GivenEntityRepository_WhenGetCount_ShouldReturnEntityCount()
        {
            // arrange
            var repository = GetRepository();
            var users = new List<User>
            {
                UserMock.Get(),
                UserMock.Get(),
                UserMock.Get()
            };

            foreach (var user in users)
            {
                await repository.AddAsync(user, CancellationToken.None);
            }

            // act
            var result = await repository.GetCountAsync(CancellationToken.None);

            // assert
            result.Should().Be(3);
        }

        private IRepository<User, UserId, Guid> GetRepository()
        {
            var connectionString = _msSql.GetConnectionString();

            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "EntityFramework:ConnectionString", connectionString },
                        { "EntityFramework:Schema", "dbo" }
                    }
                )
                .Build();

            EntityFrameworkConnection
                .ConfigureEntityFrameworkConnection<UsersContext>(
                    services,
                    configuration,
                    "users"
                );

            services.AddTransient<
                IRepository<User, UserId, Guid>,
                GenericRepository<UsersContext, User, UserId, Guid>
            >();

            var provider = services.BuildServiceProvider();

            var context = provider.GetRequiredService<UsersContext>();
            context.Database.Migrate();

            return provider.GetRequiredService<IRepository<User, UserId, Guid>>();
        }

        public async Task InitializeAsync()
        {
            await _msSql.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _msSql.DisposeAsync().AsTask();
        }
    }
}