using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.CaloriesPer100)
                   .IsRequired();

            builder.Property(p => p.ProteinPer100)
                   .IsRequired();

            builder.Property(p => p.FatPer100)
                   .IsRequired();

            builder.Property(p => p.CarbsPer100)
                   .IsRequired();

            builder
                .HasMany(p => p.DishIngredients)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);
        }
    }
}
