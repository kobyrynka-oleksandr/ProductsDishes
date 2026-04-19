using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes.DataAccess.Postgres.Models
{
    public class DailyRationDishEntity
    {
        public Guid Id { get; set; }

        public Guid DailyRationId { get; set; }
        public DailyRationEntity DailyRation { get; set; } = null!;

        public Guid DishId { get; set; }
        public DishEntity Dish { get; set; } = null!;

        public string MealType { get; set; } = string.Empty; // "Breakfast","Lunch","Dinner"
    }
}
