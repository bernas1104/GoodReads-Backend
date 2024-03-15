using System.Text.Json;

using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.BookAggregate.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodReads.Infrastructure.EntityFramework.Contexts.Mappings
{
    public class BookMapping : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    x => x.Value,
                    value => BookId.Create(value)
                );

            builder.Property(x => x.Title)
                .HasColumnName("Title")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .IsRequired();

            builder.Property(x => x.Isbn)
                .HasColumnName("ISBN")
                .IsRequired();

            builder.Property(x => x.Author)
                .HasColumnName("Author")
                .IsRequired();

            builder.Property(x => x.Gender)
                .HasColumnName("Gender")
                .HasConversion(
                    x => x.Value,
                    value => Gender.FromValue(value)
                )
                .IsRequired();

            builder.OwnsOne(
                x => x.MeanScore,
                x => {
                    x.WithOwner().HasForeignKey("BookId");

                    x.ToTable("MeanScores");

                    x.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    x.HasKey("Id");

                    x.Ignore(x => x.Value);

                    x.Property(x => x.Scores)
                        .HasColumnName("Scores")
                        .IsRequired()
                        .HasConversion(
                            x => JsonSerializer.Serialize(x, JsonSerializerOptions.Default),
                            x => JsonSerializer.Deserialize<Dictionary<int, int>>(
                                x,
                                JsonSerializerOptions.Default
                            )!
                        );
                }
            );

            builder.OwnsOne(
                x => x.BookData,
                x => {
                    x.WithOwner().HasForeignKey("BookId");

                    x.ToTable("BookDatas");

                    x.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    x.HasKey("Id");

                    x.Property(x => x.Publisher)
                        .HasColumnName("Publisher")
                        .IsRequired();

                    x.Property(x => x.YearOfPublication)
                        .HasColumnName("YearOfPublication")
                        .IsRequired();

                    x.Property(x => x.Pages)
                        .HasColumnName("Pages")
                        .IsRequired();
                }
            );

            /*
            builder.Property(x => x.Cover)
                .HasColumnName("Cover")
                .IsRequired(false);
            */

            builder.OwnsMany(
                x => x.RatingIds,
                y => {
                    y.WithOwner().HasForeignKey("BookId");

                    y.ToTable("BookRatingIds");

                    y.HasKey("Id");

                    y.Property(z => z.Value)
                        .ValueGeneratedNever()
                        .HasColumnName("RatingId");
                }
            );

            builder.Metadata.FindNavigation(nameof(Book.RatingIds))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => x.Isbn).IsUnique();

            AggregateRootMapping<Book, BookId, Guid>
                .ConfigureAggregateRoot(builder);
        }
    }
}