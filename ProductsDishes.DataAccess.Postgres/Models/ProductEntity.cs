using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Models
{
    public class ProductEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal CaloriesPer100 { get; set; }
        public decimal ProteinPer100 { get; set; }
        public decimal FatPer100 { get; set; }
        public decimal CarbsPer100 { get; set; }

        public List<DishIngradientEntity> DishIngredients { get; set; } = [];
    }
}
