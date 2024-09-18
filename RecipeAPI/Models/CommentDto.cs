using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Models
{
    public class CommentDto
    {
        public int? Id { get; set; } // Nullable for creation

        [Required]
        public string Content { get; set; }

        [Required]
        public int RecipeId { get; set; }

        public string AuthorId { get; set; } // Optional for creation; typically populated by the server

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
