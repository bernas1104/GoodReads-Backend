using GoodReads.Application;
using GoodReads.Application.Common.Behaviours;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodReads.IOC.OptionsInjection
{
    public static class ConfigureBindingsMediatR
    {
        public static void RegisterBindings(
            IServiceCollection services,
            IConfiguration configuration
        )
        {
            //

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehaviour<,>)
            );

            services.AddMediatR(
                cfg => cfg.RegisterServicesFromAssembly(
                    new AssemblyReference().GetAssembly()
                )
            );
        }
    }
}