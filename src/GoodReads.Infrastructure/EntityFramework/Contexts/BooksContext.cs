using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Infrastructure.EntityFramework.Contexts.Mappings;

using Microsoft.EntityFrameworkCore;

namespace GoodReads.Infrastructure.EntityFramework.Contexts
{
    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; private set; }

        public BooksContext(DbContextOptions<BooksContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("books");
            modelBuilder.ApplyConfiguration(new BookMapping());
        }
    }
}