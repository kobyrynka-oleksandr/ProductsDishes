using Microsoft.EntityFrameworkCore;
using ProductsDishes.DataAccess.Postgres.Configurations;
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
        public DbSet<Models.ProductEntity> Products { get; set; } = null!;
        public DbSet<Models.DishEntity> Dishes { get; set; } = null!;
        public DbSet<Models.DishIngradientEntity> DishIngredients { get; set; } = null!;
        public DbSet<Models.UserEntity> Users { get; set; } = null!;
        public DbSet<Models.DailyRationEntity> DailyRations { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new DishConfiguration());
            modelBuilder.ApplyConfiguration(new DishIngradientConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DailyRationConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
