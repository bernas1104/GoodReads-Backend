using System.Text.RegularExpressions;

using Bogus;

using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
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
        [GeneratedRegex("#ConnectionString#")]
        private static partial Regex ConnectionStringRegex();
        private static string AddConnectionString(string input, string connectionString) =>
            ConnectionStringRegex().Replace(input, connectionString);

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

            user.UpdateName(new Faker().Person.FullName);

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
                page: 3,
                pageSize: 1,
                CancellationToken.None
            );

            // assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
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

            SetupAppsettings(connectionString);

            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
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

        private static void SetupAppsettings(string connectionString)
        {
            var stream = new StreamReader("appsettings.Test.json");
            var content = stream.ReadToEnd();
            stream.Close();

            content = AddConnectionString(content, connectionString);

            StreamWriter writer = new StreamWriter("appsettings.Test.json");
            writer.Write(content);
            writer.Close();
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