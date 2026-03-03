using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Models
{
    public class DailyRationEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public DateOnly Date { get; set; }

        public List<DishEntity> Dishes { get; set; } = [];
    }

}
