using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductsDishes.DataAccess.Postgres.Repositories
{
    public class DishesRepository
    {
        private readonly ProductsDishesDbContext _dbContext;

        public DishesRepository(ProductsDishesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DishEntity>> Get()
        {
            return await _dbContext.Dishes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DishEntity>> GetWithProductsAsync()
        {
            return await _dbContext.Dishes
                .AsNoTracking()
                .Include(d => d.Ingredients)
                    .ThenInclude(di => di.Product)
                .ToListAsync();
        }

        public async Task<DishEntity?> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(id));

            return await _dbContext.Dishes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DishEntity>> GetByName(string name)
        {
            var query = _dbContext.Dishes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.Name.Contains(name));

            return await query.ToListAsync();
        }

        public async Task<List<DishEntity>> GetByPage(int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.Dishes
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<DishEntity>> GetByNamePaged(string name, int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var query = _dbContext.Dishes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.Name.Contains(name));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<DishEntity?> GetWithIngredientsAsync(Guid id)
        {
            return await _dbContext.Dishes
                .AsNoTracking()
                .Include(d => d.Ingredients)
                    .ThenInclude(di => di.Product)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task Add(Guid id, string name, string description, List<DishIngradientEntity> ingredients)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Dish name is required.", nameof(name));

            ValidateIngredients(ingredients);

            var exists = await _dbContext.Dishes.AnyAsync(d => d.Id == id);
            if (exists)
                throw new InvalidOperationException("Dish with the same id already exists.");

            var nameExists = await _dbContext.Dishes.AnyAsync(d => d.Name == name);
            if (nameExists)
                throw new InvalidOperationException("Dish with the same name already exists.");

            var dishEntity = new DishEntity
            {
                Id = id,
                Name = name.Trim(),
                Description = description?.Trim() ?? string.Empty,
                Ingredients = ingredients.Select(i => new DishIngradientEntity
                {
                    DishId = id,
                    ProductId = i.ProductId,
                    QuantityGrams = i.QuantityGrams
                }).ToList()
            };

            await _dbContext.AddAsync(dishEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, string name, string description, List<DishIngradientEntity> ingredients)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Dish name is required.", nameof(name));

            ValidateIngredients(ingredients);

            var dishExists = await _dbContext.Dishes.AnyAsync(d => d.Id == id);
            if (!dishExists)
                throw new InvalidOperationException("Dish to update was not found.");

            var nameExists = await _dbContext.Dishes
                .AnyAsync(d => d.Id != id && d.Name == name);
            if (nameExists)
                throw new InvalidOperationException("Another dish with the same name already exists.");

            await _dbContext.Dishes
                .Where(d => d.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.Name, name.Trim())
                    .SetProperty(d => d.Description, description.Trim() ?? string.Empty));

            var existing = _dbContext.DishIngredients.Where(di => di.DishId == id);
            _dbContext.DishIngredients.RemoveRange(existing);

            var distinctIngredients = ingredients
                .GroupBy(i => i.ProductId)
                .Select(g => g.Last())
                .ToList();

            foreach (var src in distinctIngredients)
            {
                var ing = new DishIngradientEntity
                {
                    DishId = id,
                    ProductId = src.ProductId,
                    QuantityGrams = src.QuantityGrams
                };

                _dbContext.DishIngredients.Add(ing);
            }

            await _dbContext.SaveChangesAsync();
        }


        public async Task Delete(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Dish id must not be empty.", nameof(id));

            var affected = await _dbContext.Dishes
                .Where(d => d.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
                throw new InvalidOperationException("Dish to delete was not found.");
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbContext.Dishes
                .AnyAsync(d => d.Name == name);
        }

        private static void ValidateIngredients(List<DishIngradientEntity> ingredients)
        {
            foreach (var ing in ingredients)
            {
                if (ing.ProductId == Guid.Empty)
                    throw new ArgumentException("Ingredient must reference a product.", nameof(ingredients));

                if (ing.QuantityGrams <= 0)
                    throw new ArgumentException("Ingredient quantity must be greater than zero.", nameof(ingredients));
            }
        }
    }
}
