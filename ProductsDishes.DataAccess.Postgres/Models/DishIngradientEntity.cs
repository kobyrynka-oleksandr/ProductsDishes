using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Models
{
    public class DishIngradientEntity
    {
        public Guid DishId { get; set; }
        public DishEntity Dish { get; set; } = null!;

        public Guid ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;

        public decimal QuantityGrams { get; set; }
    }
}
