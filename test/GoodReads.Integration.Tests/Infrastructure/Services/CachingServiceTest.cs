using GoodReads.Application.Common.Services;
using GoodReads.Infrastructure.Services;
using GoodReads.IOC.OptionsInjection;

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StackExchange.Redis;

using Testcontainers.Redis;

namespace GoodReads.Integration.Tests.Infrastructure.Services
{
    public partial class CachingServiceTest : IAsyncLifetime
    {
        private readonly RedisContainer _redis = new RedisBuilder()
            .Build();
        private readonly ILogger<CachingService> _logger;

        public CachingServiceTest()
        {
            _logger = Substitute.For<ILogger<CachingService>>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenGetAsync_WhenValueDoesNotExist_ShouldReturnNull(
            bool redisEnabled
        )
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var cachingService = GetCachingService(redisEnabled);

            // act
            var response = await cachingService.GetAsync<object>(
                key,
                CancellationToken.None
            );

            // assert
            response.Should().BeNull();

            _logger.DidNotReceive()
                .Log(
                    Arg.Is<LogLevel>(l => l == LogLevel.Error),
                    Arg.Any<EventId>(),
                    Arg.Is<object>(
                        s => s.ToString()!
                            .Contains(
                                "[CachingService] Error while " +
                                    "trying to get requested value from cache. Error: "
                            )
                    ),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<object, Exception?, string>>()
                );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenGetAsync_WhenValueExists_ShouldReturnValue(
            bool redisEnabled
        )
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var value = new Dictionary<string, string>{{ "Foo", "Bar" }};
            var cachingService = GetCachingService(redisEnabled);

            await cachingService.SetAsync(key, value, null, CancellationToken.None);

            // act
            var response = await cachingService
                .GetAsync<Dictionary<string, string>>(key, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!["Foo"].Should().Be("Bar");
        }

        private ICachingService GetCachingService(bool redisEnabled)
        {
            var connectionString = _redis.GetConnectionString();

            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "Redis:RedisConnectionString", connectionString },
                        { "Redis:RedisEnabled", redisEnabled.ToString().ToLower() },
                        { "Redis:CacheDurationInMinutes", null }
                    }
                )
                .Build();

            services.AddSingleton<IConnectionMultiplexer>(
                provider =>
                {
                    var configuration = ConfigurationOptions.Parse(connectionString);
                    configuration.AbortOnConnectFail = false;
                    configuration.ConnectTimeout = 5000;
                    return ConnectionMultiplexer.Connect(configuration);
                }
            );

            services.AddOptions<RedisCacheOptions>()
                .Configure<IServiceProvider>((options, provider) => {
                    options.ConnectionMultiplexerFactory = () =>
                        Task.FromResult(
                            provider.GetService<IConnectionMultiplexer>()
                        )!;
                });

            ConfigureBindingsCaching.RegisterBindings(services, configuration);

            services.AddTransient(x => _logger);

            var provider = services.BuildServiceProvider();

            var cachingService = provider.GetRequiredService<ICachingService>();

            return cachingService;
        }

        public async Task InitializeAsync()
        {
            await _redis.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _redis.DisposeAsync().AsTask();
        }
    }
}