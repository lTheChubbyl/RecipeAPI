using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Models
{
    public class DishType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<RecipeDishType> RecipeDishTypes { get; set; }
    }
}
