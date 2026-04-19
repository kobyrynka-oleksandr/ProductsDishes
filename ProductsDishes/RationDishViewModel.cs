using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsDishes
{
    public class RationDishViewModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal TotalCalories { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalFat { get; set; }
        public decimal TotalCarbs { get; set; }
        public Guid DishId { get; set; }
        public string MealType { get; set; } = string.Empty;
    }
}
