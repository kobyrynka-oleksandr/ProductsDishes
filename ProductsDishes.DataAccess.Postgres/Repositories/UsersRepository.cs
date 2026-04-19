using Microsoft.EntityFrameworkCore;
using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Repositories
{
    public class UsersRepository
    {
        private readonly ProductsDishesDbContext _dbContext;

        public UsersRepository(ProductsDishesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserEntity>> Get()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserEntity?> GetById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(id));

            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<UserEntity>> GetByName(string name)
        {
            var query = _dbContext.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(u => u.Name.Contains(name));

            return await query.ToListAsync();
        }

        public async Task<List<UserEntity>> GetByPage(int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _dbContext.Users
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<UserEntity>> GetByNamePaged(string name, int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var query = _dbContext.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(u => u.Name.Contains(name));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Add(Guid id, string name, int age, decimal heightCm,
    decimal weightKg, string gender, string activityLevel, string goal)
        {
            ValidateUser(id, name, age, heightCm, weightKg, gender, activityLevel, goal);

            var exists = await _dbContext.Users.AnyAsync(u => u.Id == id);
            if (exists)
                throw new InvalidOperationException("User with the same id already exists.");

            var nameExists = await _dbContext.Users.AnyAsync(u => u.Name == name);
            if (nameExists)
                throw new InvalidOperationException("User with the same name already exists.");

            var user = new UserEntity
            {
                Id = id,
                Name = name.Trim(),
                Age = age,
                HeightCm = heightCm,
                WeightKg = weightKg,
                Gender = gender.Trim(),
                ActivityLevel = activityLevel.Trim(),
                Goal = goal.Trim()
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, string name, int age, decimal heightCm,
            decimal weightKg, string gender, string activityLevel, string goal)
        {
            ValidateUser(id, name, age, heightCm, weightKg, gender, activityLevel, goal);

            if (!await _dbContext.Users.AnyAsync(u => u.Id == id))
                throw new InvalidOperationException("User to update was not found.");

            var nameExists = await _dbContext.Users
                .AnyAsync(u => u.Id != id && u.Name == name);
            if (nameExists)
                throw new InvalidOperationException("Another user with the same name already exists.");

            await _dbContext.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.Name, name.Trim())
                    .SetProperty(u => u.Age, age)
                    .SetProperty(u => u.HeightCm, heightCm)
                    .SetProperty(u => u.WeightKg, weightKg)
                    .SetProperty(u => u.Gender, gender.Trim())
                    .SetProperty(u => u.ActivityLevel, activityLevel.Trim())
                    .SetProperty(u => u.Goal, goal.Trim()));
        }

        public async Task Delete(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(id));

            var affected = await _dbContext.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
                throw new InvalidOperationException("User to delete was not found.");
        }

        public async Task<List<UserEntity>> GetAllAsync()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        private static void ValidateUser(Guid id, string name, int age,
            decimal heightCm, decimal weightKg, string gender,
            string activityLevel, string goal)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("User id must not be empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("User name is required.", nameof(name));

            if (age < 1 || age > 140)
                throw new ArgumentOutOfRangeException(nameof(age));

            if (heightCm < 50 || heightCm > 300)
                throw new ArgumentOutOfRangeException(nameof(heightCm));

            if (weightKg < 20 || weightKg > 500)
                throw new ArgumentOutOfRangeException(nameof(weightKg));

            if (string.IsNullOrWhiteSpace(gender) ||
                (gender.Trim().ToLower() != "male" && gender.Trim().ToLower() != "female"))
                throw new ArgumentException("Gender must be 'Male' or 'Female'.", nameof(gender));

            var validActivity = new[] { "Sedentary", "Light", "Moderate", "Active", "Very Active" };
            if (!validActivity.Contains(activityLevel.Trim()))
                throw new ArgumentException("Invalid activity level.", nameof(activityLevel));

            var validGoals = new[] { "Lose weight", "Maintain weight", "Gain weight" };
            if (!validGoals.Contains(goal.Trim()))
                throw new ArgumentException("Invalid goal.", nameof(goal));
        }
    }
}
