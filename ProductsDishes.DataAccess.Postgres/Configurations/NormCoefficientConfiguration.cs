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
    public class NormCoefficientConfiguration : IEntityTypeConfiguration<NormCoefficientEntity>
    {
        public void Configure(EntityTypeBuilder<NormCoefficientEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Goal).IsUnique();
            builder.Property(x => x.Goal).IsRequired().HasMaxLength(50);
            builder.Property(x => x.MinCoefficient).IsRequired();
            builder.Property(x => x.MaxCoefficient).IsRequired();

            builder.HasData(
                new NormCoefficientEntity
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                    Goal = "Lose weight",
                    MinCoefficient = 0.85m,
                    MaxCoefficient = 1.05m
                },
                new NormCoefficientEntity
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"),
                    Goal = "Maintain weight",
                    MinCoefficient = 0.90m,
                    MaxCoefficient = 1.10m
                },
                new NormCoefficientEntity
                {
                    Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"),
                    Goal = "Gain weight",
                    MinCoefficient = 0.95m,
                    MaxCoefficient = 1.15m
                }
            );
        }
    }
}
