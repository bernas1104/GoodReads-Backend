using GoodReads.Domain.Common.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodReads.Infrastructure.EntityFramework.Contexts.Mappings
{
    public static class AggregateRootMapping<TAggregate, TId, TIdType>
        where TAggregate : AggregateRoot<TId, TIdType>
        where TId : AggregateRootId<TIdType>
    {
        public static void ConfigureAggregateRoot(
            EntityTypeBuilder<TAggregate> builder
        )
        {
            builder.Property(x => x.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();

            builder.Property(x => x.DeletedAt)
                .HasColumnName("DeletedAt")
                .IsRequired(false);
        }
    }
}