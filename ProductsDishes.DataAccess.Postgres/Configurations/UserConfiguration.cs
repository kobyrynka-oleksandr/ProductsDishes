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
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(u => u.Age)
                   .IsRequired();

            builder.Property(u => u.HeightCm)
                   .IsRequired()
                   .HasPrecision(5, 2);

            builder.Property(u => u.WeightKg)
                   .IsRequired()
                   .HasPrecision(5, 2);

            builder.Property(u => u.Gender)
                   .IsRequired()
                   .HasMaxLength(10);

            builder
                .HasMany(u => u.DailyRations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);
        }
    }
}
