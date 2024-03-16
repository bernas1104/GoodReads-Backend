using FluentValidation;

using GoodReads.Application;

using Microsoft.Extensions.DependencyInjection;

namespace GoodReads.IOC.OptionsInjection
{
    public static class ConfigureBindingsValidator
    {
        public static void RegisterBindings(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(new AssemblyReference().GetAssembly());
        }
    }
}