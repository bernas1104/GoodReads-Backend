using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodReads.Infrastructure.EntityFramework.Contexts.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    x => x.Value,
                    value => UserId.Create(value)
                );

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(x => x.Email)
                .HasColumnName("Email")
                .IsRequired();

            builder.OwnsMany(
                x => x.RatingIds,
                y => {
                    y.WithOwner().HasForeignKey("UserId");

                    y.ToTable("UserRatingIds");

                    y.HasKey("Id");

                    y.Property(z => z.Value)
                        .ValueGeneratedNever()
                        .HasColumnName("RatingId");
                }
            );

            builder.Metadata.FindNavigation(nameof(User.RatingIds))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            AggregateRootMapping<User, UserId, Guid>
                .ConfigureAggregateRoot(builder);
        }
    }
}