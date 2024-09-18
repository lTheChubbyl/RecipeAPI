using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Models
{
    public class RecipeDishType
    {
        [Key]
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int DishTypeId { get; set; }
        public DishType DishType { get; set; }
    }
}
