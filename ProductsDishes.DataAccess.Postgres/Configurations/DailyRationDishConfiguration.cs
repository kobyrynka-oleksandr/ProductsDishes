using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsDishes.DataAccess.Postgres.Models;

namespace ProductsDishes.DataAccess.Postgres.Configurations
{
    public class DailyRationDishConfiguration : IEntityTypeConfiguration<DailyRationDishEntity>
    {
        public void Configure(EntityTypeBuilder<DailyRationDishEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MealType)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasOne(x => x.DailyRation)
                   .WithMany(r => r.RationDishes)
                   .HasForeignKey(x => x.DailyRationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Dish)
                   .WithMany()
                   .HasForeignKey(x => x.DishId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}