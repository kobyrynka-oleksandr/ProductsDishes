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
    public class DailyRationConfiguration : IEntityTypeConfiguration<DailyRationEntity>
    {
        public void Configure(EntityTypeBuilder<DailyRationEntity> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Date)
                   .IsRequired();

            builder
                .HasOne(r => r.User)
                .WithMany(u => u.DailyRations)
                .HasForeignKey(r => r.UserId);

            builder
                .HasMany(r => r.Dishes)
                .WithMany(d => d.DailyRations)
                .UsingEntity(j =>
                {
                    j.ToTable("DailyRationDishes");
                });
        }
    }
}
