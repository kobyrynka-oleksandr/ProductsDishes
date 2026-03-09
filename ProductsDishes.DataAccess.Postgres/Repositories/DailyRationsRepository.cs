using Microsoft.EntityFrameworkCore;
using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Repositories
{
    public class DailyRationsRepository
    {
        private readonly ProductsDishesDbContext _dbContext;

        public DailyRationsRepository(ProductsDishesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DailyRationEntity>> Get()
        {
            return await _dbContext.DailyRations
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DailyRationEntity>> GetWithNavigationAsync()
        {
            return await _dbContext.DailyRations
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Dishes)
                .ToListAsync();
        }

        public async Task<DailyRationEntity?> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Daily ration id must not be empty.", nameof(id));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<DailyRationEntity?> GetWithNavigationByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Daily ration id must not be empty.", nameof(id));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<DailyRationEntity>> GetByUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Include(r => r.Dishes)
                .ToListAsync();
        }

        public async Task<List<DailyRationEntity>> GetByDateAsync(DateOnly date)
        {
            return await _dbContext.DailyRations
                .AsNoTracking()
                .Where(r => r.Date == date)
                .Include(r => r.User)
                .Include(r => r.Dishes)
                .ToListAsync();
        }

        public async Task<List<DailyRationEntity>> GetByUserAndDateAsync(Guid userId, DateOnly date)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .Where(r => r.UserId == userId && r.Date == date)
                .Include(r => r.Dishes)
                .ToListAsync();
        }

        public async Task<List<DailyRationEntity>> GetByPage(int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .OrderBy(r => r.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<DailyRationEntity>> GetByUserPagedAsync(Guid userId, int page, int pageSize)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Guid id, Guid userId, DateOnly date, List<Guid> dishIds)
        {
            ValidateRation(id, userId, date, dishIds);

            var exists = await _dbContext.DailyRations.AnyAsync(r => r.Id == id);
            if (exists)
                throw new InvalidOperationException("Daily ration with the same id already exists.");

            if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
                throw new InvalidOperationException("User does not exist.");

            var ration = new DailyRationEntity
            {
                Id = id,
                UserId = userId,
                Date = date,
                Dishes = dishIds.Select(d => new DishEntity { Id = d }).ToList()
            };

            await _dbContext.DailyRations.AddAsync(ration);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, Guid userId, DateOnly date, List<Guid> dishIds)
        {
            ValidateRation(id, userId, date, dishIds);

            var ration = await _dbContext.DailyRations
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (ration == null)
                throw new InvalidOperationException("Daily ration not found.");

            ration.Dishes.Clear();

            foreach (var dishId in dishIds)
            {
                if (!await _dbContext.Dishes.AnyAsync(d => d.Id == dishId))
                    throw new InvalidOperationException($"Dish {dishId} does not exist.");

                ration.Dishes.Add(new DishEntity { Id = dishId });
            }

            ration.UserId = userId;
            ration.Date = date;

            await _dbContext.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Daily ration id must not be empty.", nameof(id));

            var affected = await _dbContext.DailyRations
                .Where(r => r.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
                throw new InvalidOperationException("Daily ration to delete was not found.");
        }

        private static void ValidateRation(Guid id, Guid userId, DateOnly date, List<Guid> dishIds)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Daily ration id must not be empty.", nameof(id));

            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));

            if (dishIds == null || !dishIds.Any())
                throw new ArgumentException("Daily ration must contain at least one dish.", nameof(dishIds));

            if (dishIds.Any(d => d == Guid.Empty))
                throw new ArgumentException("Dish ids must not be empty.", nameof(dishIds));

            if (dishIds.Count != dishIds.Distinct().Count())
                throw new ArgumentException("Dish ids must be unique.", nameof(dishIds));
        }
    }
}
