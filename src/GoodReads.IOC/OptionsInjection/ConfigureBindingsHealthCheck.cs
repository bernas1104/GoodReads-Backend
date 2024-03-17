using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodReads.IOC.OptionsInjection
{
    public static class ConfigureBindingsHealthCheck
    {
        public static void RegisterBindings(
            IServiceCollection services,
            IConfiguration configuration
        )
        {
            var healthBuilder = services.AddHealthChecks()
                .AddMongoDb(configuration.GetValue<string>("Mongo:ConnectionString")!)
                .AddSqlServer(configuration.GetValue<string>("EntityFramework:ConnectionString")!);

            if (configuration.GetValue<bool>("Cache:RedisEnabled"))
            {
                healthBuilder.AddRedis(configuration.GetValue<string>("Cache:RedisConnectionString")!);
            }
        }
    }
}