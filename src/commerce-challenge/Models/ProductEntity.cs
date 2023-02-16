using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace commerce_challenge.Models
{
    public class ProductEntity
    {
        public required int ExternalId { get; init; }
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Size { get; set; }
        public required int Stars { get; set; }
    }

    public class ProductEntityMap : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExternalId)
                .IsRequired();
            builder.Property(x => x.Price)
                .IsRequired();
            builder.Property(x => x.Size)
                .IsRequired();
            builder.Property(x => x.Stars)
                .IsRequired();
        }
    }
}
