namespace RecipeAPI.Models
{
    public class RecipeDetailDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }
        public List<string> DishTypeNames { get; set; }  // Dish type names
        public List<string> ExtendedIngredients { get; set; }
        public List<string> AnalyzedInstructions { get; set; }
        public string? AuthorName { get; set; }  // Author name instead of ID
    }

}
