using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Models
{
    public class CommentDto
    {
        public int? Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int RecipeId { get; set; }

        public string AuthorId { get; set; }
        public string AuthorName { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedAtFormatted => CreatedAt.ToString("f");
    }
}
