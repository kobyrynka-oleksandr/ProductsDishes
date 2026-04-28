using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Models
{
    public class NormCoefficientEntity
    {
        public Guid Id { get; set; }
        public string Goal { get; set; } = string.Empty;
        public decimal MinCoefficient { get; set; }
        public decimal MaxCoefficient { get; set; }
    }
}
