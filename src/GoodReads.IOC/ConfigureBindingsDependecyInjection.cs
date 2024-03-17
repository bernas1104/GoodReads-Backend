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
            ConfigureBindingsEntityFramework.RegisterBindings(services, configuration);
            ConfigureBindingsMediatR.RegisterBindings(services, configuration);
            ConfigureBindingsCaching.RegisterBindings(services, configuration);
            ConfigureBindingsValidator.RegisterBindings(services);
            ConfigureBindingsHealthCheck.RegisterBindings(services, configuration);
        }
    }
}