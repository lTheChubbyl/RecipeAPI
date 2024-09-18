using System.Collections.Generic;

namespace RecipeAPI.Models
{
    public class RecipeDto
    {
        public int? Id { get; set; } // Nullable for creation
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }
        public List<int> DishTypeIds { get; set; }
        public List<string> ExtendedIngredients { get; set; }
        public List<string> AnalyzedInstructions { get; set; }
        public string? AuthorId { get; set; } // Make AuthorId optional
    }
}
