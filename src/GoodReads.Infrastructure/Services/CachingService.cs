using System.Text.Json;

using GoodReads.Application.Common.Configurations;
using GoodReads.Application.Common.Services;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoodReads.Infrastructure.Services
{
    public sealed class CachingService : ICachingService
    {
        private readonly ILogger<CachingService> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly bool _redisEnabled;

        public CachingService(
            ILogger<CachingService> logger,
            IDistributedCache distributedCache,
            IMemoryCache memoryCache,
            IOptionsSnapshot<CacheConfig> options
        )
        {
            _logger = logger;
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _redisEnabled = options.Value.RedisEnabled;
        }

        public async Task<TValue?> GetAsync<TValue>(
            string key,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var resultString = string.Empty;
                if (_redisEnabled)
                {
                    resultString = await _distributedCache.GetStringAsync(
                        key,
                        cancellationToken
                    );
                }
                else
                {
                    resultString = _memoryCache.Get(key) as string;
                }

                if (!string.IsNullOrEmpty(resultString))
                {
                    return JsonSerializer.Deserialize<TValue>(resultString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[CachingService] Error while trying to get requested " +
                        "value from cache. Error: {ErrorMessage}",
                    ex.Message
                );
            }

            return default;
        }

        public async Task SetAsync<TValue>(
            string key,
            TValue value,
            DateTimeOffset? expirationDate,
            CancellationToken cancellationToken
        )
        {
            var stringValue = JsonSerializer.Serialize(value);

            try
            {
                if (_redisEnabled)
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = expirationDate
                    };

                    await _distributedCache.SetStringAsync(
                        key,
                        stringValue,
                        options,
                        cancellationToken
                    );
                }
                else
                {
                    if (expirationDate is not null)
                    {
                        _memoryCache.Set(key, stringValue, expirationDate.Value);
                    }
                    else
                    {
                        _memoryCache.Set(key, stringValue);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[CachingService] Error while trying to set requested " +
                        "value on cache. Error: {ErrorMessage}",
                    ex.Message
                );
            }
        }

    }
}