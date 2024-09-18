using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Models
{
    public class DishTypeDto
    {
        public int? Id { get; set; } // Nullable for creation

        [Required]
        public string Name { get; set; }
    }
}
