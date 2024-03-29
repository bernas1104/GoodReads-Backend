using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Infrastructure.EntityFramework.Contexts;
using GoodReads.Infrastructure.EntityFramework.Repositories;
using GoodReads.Infrastructure.EntityFramework.Utils;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using GoodReads.Infrastructure.EntityFramework.Interceptors;

namespace GoodReads.IOC.OptionsInjection
{
    public static class ConfigureBindingsEntityFramework
    {
        public static void RegisterBindings(
            IServiceCollection services,
            IConfiguration configuration
        )
        {
            EntityFrameworkConnection
                .ConfigureEntityFrameworkConnection<UsersContext>(
                    services,
                    configuration,
                    "users"
                );

            EntityFrameworkConnection
                .ConfigureEntityFrameworkConnection<BooksContext>(
                    services,
                    configuration,
                    "books"
                );

            services.AddScoped<DomainEventsInterceptor>();

            services.AddScoped<
                IRepository<User, UserId, Guid>,
                GenericRepository<UsersContext, User, UserId, Guid>
            >();

            services.AddScoped<
                IRepository<Book, BookId, Guid>,
                GenericRepository<BooksContext, Book, BookId, Guid>
            >();
        }
    }
}