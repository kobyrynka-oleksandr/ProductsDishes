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
    public class DishIngradientConfiguration : IEntityTypeConfiguration<DishIngradientEntity>
    {
        public void Configure(EntityTypeBuilder<DishIngradientEntity> builder)
        {
            builder.HasKey(i => new { i.DishId, i.ProductId });

            builder
                .HasOne(i => i.Dish)
                .WithMany(d => d.Ingredients)
                .HasForeignKey(i => i.DishId);

            builder
                .HasOne(i => i.Product)
                .WithMany(p => p.DishIngredients)
                .HasForeignKey(i => i.ProductId);

            builder.Property(i => i.QuantityGrams)
                   .IsRequired();
        }
    }
}
