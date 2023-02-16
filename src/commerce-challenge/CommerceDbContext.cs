using System.Reflection;
using commerce_challenge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace commerce_challenge
{
    public class CommerceDbContext : DbContext
    {
        public DbSet<CartEntity> Carts { get; init; }
        public DbSet<ProductEntity> Products { get; init; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "CommerceDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(ProductEntity))!);
            base.OnModelCreating(modelBuilder);
        }
    }
}
