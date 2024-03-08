using System.Text;

using GoodReads.Application.Common.Configurations;
using GoodReads.Infrastructure.Services;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute.ExceptionExtensions;

namespace GoodReads.Unit.Tests.Infrastructure.Services
{
    public class CachingServiceTest
    {
        private readonly ILogger<CachingService> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private IOptionsSnapshot<CacheConfig>? _options;
        private CachingService? _cachingService;

        public CachingServiceTest()
        {
            _logger = Substitute.For<ILogger<CachingService>>();
            _distributedCache = Substitute.For<IDistributedCache>();
            _memoryCache = Substitute.For<IMemoryCache>();
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
            var data = "123456";

            _memoryCache.TryGetValue(
                Arg.Is<string>(k => k == key),
                out var value
            ).Returns(
                x => {
                    x[1] = data;
                    return true;
                }
            );

            _distributedCache.GetAsync(
                Arg.Is<string>(x => x == key),
                Arg.Any<CancellationToken>()
            ).Returns(Encoding.UTF8.GetBytes(data));

            InitializeCachingService(redisEnabled);

            // act
            var response = await _cachingService!.GetAsync<int>(
                key,
                CancellationToken.None
            );

            // assert
            response.Should().Be(123456);
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

            _memoryCache.TryGetValue(
                Arg.Is<string>(k => k == key),
                out var value
            ).Returns(
                x => {
                    x[1] = null;
                    return true;
                }
            );

            _distributedCache.GetAsync(
                Arg.Is<string>(x => x == key),
                Arg.Any<CancellationToken>()
            ).Returns(Encoding.UTF8.GetBytes(string.Empty));

            InitializeCachingService(redisEnabled);

            // act
            var response = await _cachingService!.GetAsync<int>(
                key,
                CancellationToken.None
            );

            // assert
            response.Should().Be(0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenGetAsync_WhenExceptionOccurs_ShouldLogError(
            bool redisEnabled
        )
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var errorMessage = "Some caching error";
            var exception = new Exception(errorMessage);

            _memoryCache.TryGetValue(
                Arg.Is<string>(k => k == key),
                out var value
            ).Throws(exception);

            _distributedCache.GetAsync(
                Arg.Is<string>(x => x == key),
                Arg.Any<CancellationToken>()
            ).ThrowsAsync(exception);

            InitializeCachingService(redisEnabled);

            // act
            var response = await _cachingService!.GetAsync<int>(
                key,
                CancellationToken.None
            );

            // assert
            response.Should().Be(0);

            _logger.ShouldHaveLoggedError(
                "[CachingService] Error while trying to get requested " +
                    $"value from cache. Error: {errorMessage}"
            );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenSetAsync_WhenExpirationDateNotGiven_ShouldSetValueOnCache(
            bool redisEnabled
        )
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var value = 123456;

            InitializeCachingService(redisEnabled);

            // act
            await _cachingService!.SetAsync(
                key,
                value,
                null,
                CancellationToken.None
            );

            // assert
            if (redisEnabled)
            {
                await _distributedCache.Received()
                    .SetAsync(
                        Arg.Is<string>(k => k == key),
                        Arg.Any<byte[]>(),
                        Arg.Any<DistributedCacheEntryOptions>(),
                        Arg.Any<CancellationToken>()
                    );
            }
            else
            {
                _memoryCache.Received().CreateEntry(Arg.Is<string>(k => k == key));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenSetAsync_WhenExpirationDateGiven_ShouldSetValueOnCacheWithOffset(
            bool redisEnabled
        )
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var value = 123456;
            var expirationDate = DateTimeOffset.UtcNow.AddMinutes(1);

            InitializeCachingService(redisEnabled);

            // act
            await _cachingService!.SetAsync(
                key,
                value,
                null,
                CancellationToken.None
            );

            // assert
            if (redisEnabled)
            {
                await _distributedCache.Received()
                    .SetAsync(
                        Arg.Is<string>(k => k == key),
                        Arg.Any<byte[]>(),
                        Arg.Any<DistributedCacheEntryOptions>(),
                        Arg.Any<CancellationToken>()
                    );
            }
            else
            {
                _memoryCache.Received().CreateEntry(Arg.Is<string>(k => k == key));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenSetAsync_WhenErrorOccurs_ShouldLogError(
            bool redisEnabled
        )
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var value = 123456;
            var errorMessage = "Some caching error";
            var exception = new Exception(errorMessage);

            InitializeCachingService(redisEnabled);

            _distributedCache.SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>()
            ).ThrowsAsync(exception);

            _memoryCache.CreateEntry(Arg.Any<string>()).Throws(exception);

            // act
            await _cachingService!.SetAsync(
                key,
                value,
                null,
                CancellationToken.None
            );

            // assert
            _logger.ShouldHaveLoggedError(
                "[CachingService] Error while trying to set requested " +
                    $"value on cache. Error: {errorMessage}"
            );
        }

        private void InitializeCachingService(bool redisEnabled)
        {
            _options = Substitute.For<IOptionsSnapshot<CacheConfig>>();
            _options.Value.Returns(new CacheConfig { RedisEnabled = redisEnabled });

            _cachingService = new (
                _logger,
                _distributedCache,
                _memoryCache,
                _options
            );
        }
    }
}