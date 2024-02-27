using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Infrastructure.EntityFramework.Contexts.Mappings;

using Microsoft.EntityFrameworkCore;

namespace GoodReads.Infrastructure.EntityFramework.Contexts
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; private set; }

        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("users");
            modelBuilder.ApplyConfiguration(new UserMapping());
        }
    }
}