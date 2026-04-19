using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal HeightCm { get; set; }
        public decimal WeightKg { get; set; }
        public string Gender { get; set; } = string.Empty;

        public string ActivityLevel { get; set; } = "Sedentary";
        public string Goal { get; set; } = "Maintain weight";

        public List<DailyRationEntity> DailyRations { get; set; } = [];
    }
}
