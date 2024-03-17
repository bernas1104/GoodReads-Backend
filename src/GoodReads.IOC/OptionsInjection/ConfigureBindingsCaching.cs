using GoodReads.Application.Common.Configurations;
using GoodReads.Application.Common.Services;
using GoodReads.Infrastructure.Services;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace GoodReads.IOC.OptionsInjection
{
    public static class ConfigureBindingsCaching
    {
        public static void RegisterBindings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var section = configuration.GetSection(CacheConfig.Cache);
            var connection = section.Get<CacheConfig>();
            var connectionString = connection?.RedisConnectionString;

            services.Configure<CacheConfig>(section);

            services.AddStackExchangeRedisCache(
                options => options.Configuration = configuration
                    .GetConnectionString(connectionString!)!
            );

            services.AddMemoryCache();

            services.AddScoped<ICachingService, CachingService>();
        }
    }
}