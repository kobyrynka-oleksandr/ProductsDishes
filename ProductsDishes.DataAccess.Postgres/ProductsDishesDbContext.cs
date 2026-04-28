using Microsoft.EntityFrameworkCore;
using ProductsDishes.DataAccess.Postgres.Configurations;
using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres
{
    public class ProductsDishesDbContext : DbContext
    {
        public ProductsDishesDbContext(DbContextOptions<ProductsDishesDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductEntity> Products { get; set; } = null!;
        public DbSet<DishEntity> Dishes { get; set; } = null!;
        public DbSet<DishIngradientEntity> DishIngredients { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<DailyRationEntity> DailyRations { get; set; } = null!;
        public DbSet<DailyRationDishEntity> DailyRationDishes { get; set; } = null!;
        public DbSet<NormCoefficientEntity> NormCoefficients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new DishConfiguration());
            modelBuilder.ApplyConfiguration(new DishIngradientConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DailyRationConfiguration());
            modelBuilder.ApplyConfiguration(new DailyRationDishConfiguration());
            modelBuilder.ApplyConfiguration(new NormCoefficientConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
