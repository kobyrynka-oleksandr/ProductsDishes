using Microsoft.EntityFrameworkCore;
using ProductsDishes.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Repositories
{
    public class NormCoefficientsRepository
    {
        private readonly ProductsDishesDbContext _context;

        public NormCoefficientsRepository(ProductsDishesDbContext context)
            => _context = context;

        public async Task<NormCoefficientEntity?> GetByUserGoalAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Join(
                    _context.NormCoefficients,
                    u => u.Goal,
                    nc => nc.Goal,
                    (u, nc) => nc)
                .FirstOrDefaultAsync();
        }

        public async Task<NormCoefficientEntity?> GetByGoalAsync(string goal)
            => await _context.NormCoefficients.FirstOrDefaultAsync(nc => nc.Goal == goal);
    }
}
