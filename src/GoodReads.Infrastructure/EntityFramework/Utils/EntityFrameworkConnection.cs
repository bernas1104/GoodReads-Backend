using GoodReads.Infrastructure.EntityFramework.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodReads.Infrastructure.EntityFramework.Utils
{
    public static class EntityFrameworkConnection
    {
        public static void ConfigureEntityFrameworkConnection<TContext>(
            IServiceCollection services,
            IConfiguration configuration,
            string schema
        ) where TContext : DbContext
        {
            var section = configuration.GetSection(
                EntityFrameworkConnectionOptions.EntityFramework
            );

            var connection = section.Get<EntityFrameworkConnectionOptions>();
            var connectionString = connection?.ConnectionString;

            services.Configure<EntityFrameworkConnectionOptions>(section);

            services.AddDbContext<TContext>(
                opt => opt.UseSqlServer(
                    connectionString,
                    o => o.MigrationsHistoryTable(
                        tableName: HistoryRepository.DefaultTableName,
                        schema: schema
                    )
                )
            );

            services.AddSqlServer<TContext>(connectionString);
        }
    }
}