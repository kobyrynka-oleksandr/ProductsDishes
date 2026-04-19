using Microsoft.EntityFrameworkCore;
using ProductsDishes.DataAccess.Postgres.Models;

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

        public async Task<List<DailyRationEntity>> GetByUserAndDateAsync(Guid userId, DateOnly date)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));

            return await _dbContext.DailyRations
                .AsNoTracking()
                .Where(r => r.UserId == userId && r.Date == date)
                .Include(r => r.RationDishes)
                    .ThenInclude(rd => rd.Dish)
                        .ThenInclude(d => d.Ingredients)
                            .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task SaveRationAsync(Guid userId, DateOnly date,
            List<(Guid DishId, string MealType)> entries)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));
            if (!entries.Any())
                throw new ArgumentException("Ration must contain at least one dish.", nameof(entries));

            var old = await _dbContext.DailyRations
                .Include(r => r.RationDishes)
                .Where(r => r.UserId == userId && r.Date == date)
                .ToListAsync();

            _dbContext.DailyRations.RemoveRange(old);
            await _dbContext.SaveChangesAsync();

            var ration = new DailyRationEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                RationDishes = entries.Select(e => new DailyRationDishEntity
                {
                    Id = Guid.NewGuid(),
                    DishId = e.DishId,
                    MealType = e.MealType
                }).ToList()
            };

            await _dbContext.DailyRations.AddAsync(ration);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<string>> GetAllGroupedAsync()
        {
            var rows = await _dbContext.DailyRations
                .AsNoTracking()
                .Include(r => r.User)
                .Select(r => new { r.Date, UserName = r.User != null ? r.User.Name : "Unknown" })
                .ToListAsync();

            return rows
                .DistinctBy(x => (x.Date, x.UserName))
                .OrderByDescending(x => x.Date)
                .Select(x => $"{x.Date:yyyy-MM-dd} | {x.UserName}")
                .ToList();
        }

        public async Task DeleteByUserAndDateAsync(Guid userId, DateOnly date)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));

            await _dbContext.DailyRations
                .Where(r => r.UserId == userId && r.Date == date)
                .ExecuteDeleteAsync();
        }

        public async Task DeleteByUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(userId));

            await _dbContext.DailyRations
                .Where(r => r.UserId == userId)
                .ExecuteDeleteAsync();
        }
    }
}