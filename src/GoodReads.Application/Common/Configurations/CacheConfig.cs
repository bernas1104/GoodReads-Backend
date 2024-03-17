namespace GoodReads.Application.Common.Configurations
{
    public sealed class CacheConfig
    {
        public const string Cache = "Cache";
        public string RedisConnectionString { get; init; } = string.Empty;
        public bool RedisEnabled { get; init; }
        public int CacheDurationInMinutes { get; init; }
    }
}