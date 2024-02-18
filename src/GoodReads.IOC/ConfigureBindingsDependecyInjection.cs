using GoodReads.IOC.OptionsInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodReads.IOC
{
    public static class ConfigureBindingsDependecyInjection
    {
        public static void RegisterBindings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            ConfigureBindingsMongo.RegisterBindings(services, configuration);
        }
    }
}