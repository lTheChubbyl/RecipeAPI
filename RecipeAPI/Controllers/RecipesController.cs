using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecipesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDetailDto>>> GetRecipes()
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeDishTypes)
                .ThenInclude(rt => rt.DishType)
                .Include(r => r.Author)  // Include the Author for name mapping
                .Select(r => new RecipeDetailDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Summary = r.Summary,
                    Image = r.Image,
                    ReadyInMinutes = r.ReadyInMinutes,
                    Servings = r.Servings,
                    DishTypeNames = r.RecipeDishTypes
                        .Select(rt => rt.DishType.Name)  // Assuming DishType has a Name property
                        .ToList(),
                    ExtendedIngredients = r.ExtendedIngredients,
                    AnalyzedInstructions = r.AnalyzedInstructions,
                    AuthorName = r.Author.FirstName + " " + r.Author.LastName  // Assuming ApplicationUser has a UserName property
                })
                .ToListAsync();

            return Ok(recipes);
        }

        // GET: api/Recipes/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RecipeDetailDto>>> GetUserRecipes(string userId)
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeDishTypes)
                .ThenInclude(rt => rt.DishType)
                .Include(r => r.Author)  // Include the Author for name mapping
                .Where(r => r.AuthorId == userId)
                .Select(r => new RecipeDetailDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Summary = r.Summary,
                    Image = r.Image,
                    ReadyInMinutes = r.ReadyInMinutes,
                    Servings = r.Servings,
                    DishTypeNames = r.RecipeDishTypes
                        .Select(rt => rt.DishType.Name)  // Assuming DishType has a Name property
                        .ToList(),
                    ExtendedIngredients = r.ExtendedIngredients,
                    AnalyzedInstructions = r.AnalyzedInstructions,
                    AuthorName = r.Author.FirstName + " " + r.Author.LastName  // Assuming ApplicationUser has a UserName property
                })
                .ToListAsync();

            return Ok(recipes);
        }


        // GET: api/Recipes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDetailDto>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeDishTypes)
                .ThenInclude(rt => rt.DishType)
                .Include(r => r.Author)  // Include the Author for name mapping
                .Where(r => r.Id == id)
                .Select(r => new RecipeDetailDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Summary = r.Summary,
                    Image = r.Image,
                    ReadyInMinutes = r.ReadyInMinutes,
                    Servings = r.Servings,
                    DishTypeNames = r.RecipeDishTypes
                        .Select(rt => rt.DishType.Name)  // Assuming DishType has a Name property
                        .ToList(),
                    ExtendedIngredients = r.ExtendedIngredients,
                    AnalyzedInstructions = r.AnalyzedInstructions,
                    AuthorName = r.Author.FirstName + " " + r.Author.LastName  // Assuming ApplicationUser has a UserName property
                })
                .FirstOrDefaultAsync();

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }


        // GET: api/Recipes/dishType/{dishTypeId}
        [HttpGet("dishType/{dishTypeId}")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByDishType(int dishTypeId)
        {
            var recipes = await _context.Recipes
                .Where(r => r.RecipeDishTypes.Any(rt => rt.DishTypeId == dishTypeId))
                .Include(r => r.RecipeDishTypes)
                .ThenInclude(rt => rt.DishType)
                .Include(r => r.Author)
                .Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Summary = r.Summary,
                    Image = r.Image,
                    ReadyInMinutes = r.ReadyInMinutes,
                    Servings = r.Servings,
                    DishTypeIds = r.RecipeDishTypes.Select(rt => rt.DishTypeId).ToList(),
                    ExtendedIngredients = r.ExtendedIngredients,
                    AnalyzedInstructions = r.AnalyzedInstructions,
                    AuthorId = r.AuthorId
                })
                .ToListAsync();

            if (recipes == null || !recipes.Any())
            {
                return NotFound();
            }

            return Ok(recipes);
        }

        // GET: api/Recipes/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> SearchRecipes([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var lowerSearchTerm = searchTerm.ToLower();

            var recipes = await _context.Recipes
                .Include(r => r.RecipeDishTypes)
                .ThenInclude(rt => rt.DishType)
                .Include(r => r.Author)
                .ToListAsync();

            var filteredRecipes = recipes
                .Where(r => r.Title.ToLower().Contains(lowerSearchTerm) ||
                            r.Summary.ToLower().Contains(lowerSearchTerm) ||
                            r.ExtendedIngredients
                                .Select(e => e.ToLower())
                                .Any(e => e.Contains(lowerSearchTerm)))
                .Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Summary = r.Summary,
                    Image = r.Image,
                    ReadyInMinutes = r.ReadyInMinutes,
                    Servings = r.Servings,
                    DishTypeIds = r.RecipeDishTypes.Select(rt => rt.DishTypeId).ToList(),
                    ExtendedIngredients = r.ExtendedIngredients,
                    AnalyzedInstructions = r.AnalyzedInstructions,
                    AuthorId = r.AuthorId
                })
                .ToList();

            if (!filteredRecipes.Any())
            {
                return NotFound("No recipes found matching the search term.");
            }

            return Ok(filteredRecipes);
        }


        // POST: api/Recipes
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> PostRecipe(RecipeDto recipeDto)
        {
            var recipe = new Recipe
            {
                Title = recipeDto.Title,
                Summary = recipeDto.Summary,
                Image = recipeDto.Image,
                ReadyInMinutes = recipeDto.ReadyInMinutes,
                Servings = recipeDto.Servings,
                ExtendedIngredients = recipeDto.ExtendedIngredients,
                AnalyzedInstructions = recipeDto.AnalyzedInstructions,
                AuthorId = recipeDto.AuthorId
            };

            //// Set the AuthorId based on the currently authenticated user
            //var userId = User.Identity?.Name;
            //if (userId != null)
            //{
            //    var user = await _userManager.FindByNameAsync(userId);
            //    if (user != null)
            //    {
            //        recipe.AuthorId = user.Id;
            //    }
            //}

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Handle dish types
            if (recipeDto.DishTypeIds != null && recipeDto.DishTypeIds.Any())
            {
                foreach (var dishTypeId in recipeDto.DishTypeIds)
                {
                    _context.RecipeDishTypes.Add(new RecipeDishType
                    {
                        DishTypeId = dishTypeId,
                        RecipeId = recipe.Id
                    });
                }
                await _context.SaveChangesAsync();
            }

            var createdRecipeDto = new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Summary = recipe.Summary,
                Image = recipe.Image,
                ReadyInMinutes = recipe.ReadyInMinutes,
                Servings = recipe.Servings,
                DishTypeIds = recipeDto.DishTypeIds,
                ExtendedIngredients = recipe.ExtendedIngredients,
                AnalyzedInstructions = recipe.AnalyzedInstructions,
                AuthorId = recipe.AuthorId
            };

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, createdRecipeDto);
        }

        // PUT: api/Recipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, RecipeDto recipeDto)
        {
            if (id != recipeDto.Id)
            {
                return BadRequest();
            }

            var existingRecipe = await _context.Recipes
                .Include(r => r.RecipeDishTypes)
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (existingRecipe == null)
            {
                return NotFound();
            }

            existingRecipe.Title = recipeDto.Title ?? existingRecipe.Title;
            existingRecipe.Summary = recipeDto.Summary ?? existingRecipe.Summary;
            existingRecipe.Image = recipeDto.Image ?? existingRecipe.Image;
            existingRecipe.ReadyInMinutes = recipeDto.ReadyInMinutes != 0 ? recipeDto.ReadyInMinutes : existingRecipe.ReadyInMinutes;
            existingRecipe.Servings = recipeDto.Servings != 0 ? recipeDto.Servings : existingRecipe.Servings;
            existingRecipe.ExtendedIngredients = recipeDto.ExtendedIngredients ?? existingRecipe.ExtendedIngredients;
            existingRecipe.AnalyzedInstructions = recipeDto.AnalyzedInstructions ?? existingRecipe.AnalyzedInstructions;

            // Handle dish types
            if (recipeDto.DishTypeIds != null)
            {
                // Remove old dish types
                var existingDishTypes = _context.RecipeDishTypes.Where(rt => rt.RecipeId == id).ToList();
                _context.RecipeDishTypes.RemoveRange(existingDishTypes);

                // Add new dish types
                foreach (var dishTypeId in recipeDto.DishTypeIds)
                {
                    _context.RecipeDishTypes.Add(new RecipeDishType
                    {
                        DishTypeId = dishTypeId,
                        RecipeId = id
                    });
                }
                await _context.SaveChangesAsync();
            }

            _context.Entry(existingRecipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
