using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductsDishes.DataAccess.Postgres.Repositories
{
    public class ProductsRepository
    {
        private readonly ProductsDishesDbContext _dbContext;

        public ProductsRepository(ProductsDishesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductEntity>> Get()
        {
            return await _dbContext.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ProductEntity>> GetWithDishesAsync()
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.DishIngredients)
                    .ThenInclude(di => di.Dish)
                .ToListAsync();
        }

        public async Task<ProductEntity?> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Product id must not be empty.", nameof(id));

            return await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProductEntity>> GetByName(string name)
        {
            var query = _dbContext.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            return await query.ToListAsync();
        }

        public async Task<List<ProductEntity>> GetByPage(int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.Products
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<ProductEntity>> GetByNamePaged(string name, int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var query = _dbContext.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task Add(Guid id, string name,
            decimal caloriesPer100, decimal proteinPer100,
            decimal fatPer100, decimal carbsPer100)
        {
            ValidateProduct(id, name,
                caloriesPer100, proteinPer100,
                fatPer100, carbsPer100);

            var exists = await _dbContext.Products.AnyAsync(p => p.Id == id);
            if (exists)
                throw new InvalidOperationException("Product with the same id already exists.");

            var nameExists = await _dbContext.Products.AnyAsync(p => p.Name == name);
            if (nameExists)
                throw new InvalidOperationException("Product with the same name already exists.");

            var product = new ProductEntity
            {
                Id = id,
                Name = name.Trim(),
                CaloriesPer100 = caloriesPer100,
                ProteinPer100 = proteinPer100,
                FatPer100 = fatPer100,
                CarbsPer100 = carbsPer100
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, string name,
            decimal caloriesPer100, decimal proteinPer100,
            decimal fatPer100, decimal carbsPer100)
        {
            ValidateProduct(id, name,
                caloriesPer100, proteinPer100,
                fatPer100, carbsPer100);

            var exists = await _dbContext.Products.AnyAsync(p => p.Id == id);
            if (!exists)
                throw new InvalidOperationException("Product to update was not found.");

            var nameExists = await _dbContext.Products
                .AnyAsync(p => p.Id != id && p.Name == name);
            if (nameExists)
                throw new InvalidOperationException("Another product with the same name already exists.");

            var affected = await _dbContext.Products
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Name, name.Trim())
                    .SetProperty(p => p.CaloriesPer100, caloriesPer100)
                    .SetProperty(p => p.ProteinPer100, proteinPer100)
                    .SetProperty(p => p.FatPer100, fatPer100)
                    .SetProperty(p => p.CarbsPer100, carbsPer100));
        }

        public async Task Delete(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Product id must not be empty.", nameof(id));

            var affected = await _dbContext.Products
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
                throw new InvalidOperationException("Product to delete was not found.");
        }

        private static void ValidateProduct(Guid id, string name,
            decimal caloriesPer100, decimal proteinPer100,
            decimal fatPer100, decimal carbsPer100)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Product id must not be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            if (caloriesPer100 < 0)
                throw new ArgumentOutOfRangeException(nameof(caloriesPer100), "Calories cannot be negative.");
            if (proteinPer100 < 0)
                throw new ArgumentOutOfRangeException(nameof(proteinPer100), "Protein cannot be negative.");
            if (fatPer100 < 0)
                throw new ArgumentOutOfRangeException(nameof(fatPer100), "Fat cannot be negative.");
            if (carbsPer100 < 0)
                throw new ArgumentOutOfRangeException(nameof(carbsPer100), "Carbs cannot be negative.");

            var sumMacros = proteinPer100 + fatPer100 + carbsPer100;
            if (sumMacros > 120)
                throw new ArgumentException("Sum of macros per 100g is not realistic.", nameof(sumMacros));
        }
    }
}
