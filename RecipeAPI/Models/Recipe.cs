using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public int ReadyInMinutes { get; set; }
        public int Servings { get; set; }
        public List<string> ExtendedIngredients { get; set; }
        public List<string> AnalyzedInstructions { get; set; }

        public string? AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public ApplicationUser? Author { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<RecipeDishType> RecipeDishTypes { get; set; } = new List<RecipeDishType>();
    }
}
