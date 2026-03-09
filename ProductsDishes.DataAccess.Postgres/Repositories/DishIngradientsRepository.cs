using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductsDishes.DataAccess.Postgres.Repositories
{
    public class DishIngradientsRepository
    {
        private readonly ProductsDishesDbContext _dbContext;

        public DishIngradientsRepository(ProductsDishesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DishIngradientEntity>> Get()
        {
            return await _dbContext.DishIngredients
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DishIngradientEntity>> GetWithNavigationAsync()
        {
            return await _dbContext.DishIngredients
                .AsNoTracking()
                .Include(di => di.Dish)
                .Include(di => di.Product)
                .ToListAsync();
        }

        public async Task<DishIngradientEntity?> GetByIdsAsync(Guid dishId, Guid productId)
        {
            if (dishId == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(dishId));
            if (productId == Guid.Empty)
                throw new ArgumentException("Product id must not be empty.", nameof(productId));

            return await _dbContext.DishIngredients
                .AsNoTracking()
                .FirstOrDefaultAsync(di => di.DishId == dishId && di.ProductId == productId);
        }

        public async Task<List<DishIngradientEntity>> GetByDishAsync(Guid dishId)
        {
            if (dishId == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(dishId));

            return await _dbContext.DishIngredients
                .AsNoTracking()
                .Where(di => di.DishId == dishId)
                .Include(di => di.Product)
                .ToListAsync();
        }

        public async Task<List<DishIngradientEntity>> GetByPageAsync(int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.DishIngredients
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<DishIngradientEntity>> GetByDishPageAsync(Guid dishId, int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.DishIngredients
                .AsNoTracking()
                .Where(di => di.DishId == dishId)
                .Include(di => di.Product)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Guid dishId, Guid productId, decimal quantityGrams)
        {
            ValidateIdsAndQuantity(dishId, productId, quantityGrams);

            var exists = await _dbContext.DishIngredients
                .AnyAsync(di => di.DishId == dishId && di.ProductId == productId);
            if (exists)
                throw new InvalidOperationException("Ingredient for this dish and product already exists.");

            var dishExists = await _dbContext.Dishes.AnyAsync(d => d.Id == dishId);
            if (!dishExists)
                throw new InvalidOperationException("Dish does not exist.");

            var productExists = await _dbContext.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new InvalidOperationException("Product does not exist.");

            var entity = new DishIngradientEntity
            {
                DishId = dishId,
                ProductId = productId,
                QuantityGrams = quantityGrams
            };

            await _dbContext.DishIngredients.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid dishId, Guid productId, decimal quantityGrams)
        {
            ValidateIdsAndQuantity(dishId, productId, quantityGrams);

            var affected = await _dbContext.DishIngredients
                .Where(di => di.DishId == dishId && di.ProductId == productId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(di => di.QuantityGrams, quantityGrams));

            if (affected == 0)
                throw new InvalidOperationException("Dish ingredient to update was not found.");
        }

        public async Task DeleteAsync(Guid dishId, Guid productId)
        {
            if (dishId == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(dishId));
            if (productId == Guid.Empty)
                throw new ArgumentException("Product id must not be empty.", nameof(productId));

            var affected = await _dbContext.DishIngredients
                .Where(di => di.DishId == dishId && di.ProductId == productId)
                .ExecuteDeleteAsync();

            if (affected == 0)
                throw new InvalidOperationException("Dish ingredient to delete was not found.");
        }

        private static void ValidateIdsAndQuantity(Guid dishId, Guid productId, decimal quantityGrams)
        {
            if (dishId == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(dishId));
            if (productId == Guid.Empty)
                throw new ArgumentException("Product id must not be empty.", nameof(productId));
            if (quantityGrams <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantityGrams), "Quantity must be greater than zero.");
        }
    }
}
