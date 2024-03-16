using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Infrastructure.EntityFramework.Contexts.Mappings;
using GoodReads.Infrastructure.EntityFramework.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace GoodReads.Infrastructure.EntityFramework.Contexts
{
    public class BooksContext : DbContext
    {
        private readonly DomainEventsInterceptor _domainEventsInterceptor;
        public DbSet<Book> Books { get; private set; }

        public BooksContext(
            DbContextOptions<BooksContext> options,
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

            modelBuilder.HasDefaultSchema("books");
            modelBuilder.ApplyConfiguration(new BookMapping());
        }
    }
}