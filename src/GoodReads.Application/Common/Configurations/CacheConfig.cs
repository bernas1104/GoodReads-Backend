namespace GoodReads.Application.Common.Configurations
{
    public sealed class CacheConfig
    {
        public const string Redis = "Redis";
        public string RedisConnectionString { get; init; } = string.Empty;
        public bool RedisEnabled { get; init; } = false;
        public int CacheDurationInMinutes { get; init; }
    }
}