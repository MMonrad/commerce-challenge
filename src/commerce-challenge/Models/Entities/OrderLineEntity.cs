using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace commerce_challenge.Models.Entities
{
    public class OrderLineEntity
    {
        public required CartEntity Cart { get; init; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required decimal ProductUnitPrice { get; set; }
        public required uint Quantity { get; set; }
        public decimal TotalPrice => Quantity * ProductUnitPrice;
    }

    public class OrderLineEntityMap : IEntityTypeConfiguration<OrderLineEntity>
    {
        public void Configure(EntityTypeBuilder<OrderLineEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductId)
                .IsRequired();
            builder.Property(x => x.ProductName)
                .IsRequired();
            builder.Property(x => x.ProductUnitPrice)
                .IsRequired();
            builder.Property(x => x.Quantity)
                .IsRequired();
            builder.Ignore(x => x.TotalPrice);
        }
    }
}
