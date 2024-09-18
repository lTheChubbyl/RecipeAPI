using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Models;

namespace RecipeAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<DishType> DishTypes { get; set; } // Add DbSet for DishType
        public DbSet<RecipeDishType> RecipeDishTypes { get; set; } // Add DbSet for RecipeDishType

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the one-to-many relationship between ApplicationUser and Recipe
            builder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany() // ApplicationUser does not have a collection of Recipes
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.SetNull); // Optional: Set to null if the user is deleted

            // Configure the relationship between Recipe and Comment
            builder.Entity<Comment>()
                .HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between ApplicationUser and Comment
            builder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany() // ApplicationUser does not have a collection of Comments
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure the many-to-many relationship between Recipe and DishType
            builder.Entity<RecipeDishType>()
                .HasKey(rt => new { rt.RecipeId, rt.DishTypeId });

            builder.Entity<RecipeDishType>()
                .HasOne(rt => rt.Recipe)
                .WithMany(r => r.RecipeDishTypes)
                .HasForeignKey(rt => rt.RecipeId);

            builder.Entity<RecipeDishType>()
                .HasOne(rt => rt.DishType)
                .WithMany(d => d.RecipeDishTypes)
                .HasForeignKey(rt => rt.DishTypeId);
        }
    }
}
