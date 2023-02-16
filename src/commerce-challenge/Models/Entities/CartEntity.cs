using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace commerce_challenge.Models.Entities
{
    public class CartEntity
    {
        public required string? Email { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public ICollection<OrderLineEntity> OrderLines { get; init; } = new List<OrderLineEntity>();
        public decimal TotalAmount { get; set; }
    }

    public class CartEntityMap : IEntityTypeConfiguration<CartEntity>
    {
        public void Configure(EntityTypeBuilder<CartEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired(false);
            builder.Property(x => x.TotalAmount)
                .IsRequired();

            builder.HasMany<OrderLineEntity>()
                .WithOne(x => x.Cart);
        }
    }
}
