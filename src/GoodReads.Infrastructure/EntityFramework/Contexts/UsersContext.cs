using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Infrastructure.EntityFramework.Contexts.Mappings;
using GoodReads.Infrastructure.EntityFramework.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace GoodReads.Infrastructure.EntityFramework.Contexts
{
    public class UsersContext : DbContext
    {
        private readonly DomainEventsInterceptor _domainEventsInterceptor;
        public DbSet<User> Users { get; private set; }

        public UsersContext(
            DbContextOptions<UsersContext> options,
            DomainEventsInterceptor domainEventsInterceptor
        ) : base(options)
        {
            _domainEventsInterceptor = domainEventsInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.AddInterceptors(_domainEventsInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("users");
            modelBuilder.ApplyConfiguration(new UserMapping());
        }
    }
}