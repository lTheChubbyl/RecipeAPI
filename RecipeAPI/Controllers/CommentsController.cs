using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
        {
            var comments = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Recipe)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    RecipeId = c.RecipeId,
                    AuthorId = c.AuthorId,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForRecipe(int id)
        {
            var comments = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Recipe)
                .Where(c => c.RecipeId == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    RecipeId = c.RecipeId,
                    AuthorId = c.AuthorId,
                    CreatedAt = c.CreatedAt,
                    AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                })
                .ToListAsync(); // Return a list of comments

            if (!comments.Any())
            {
                return NotFound(new { message = "No comments found for this recipe" });
            }

            return Ok(comments);
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<CommentDto>> PostComment([FromBody] CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure that RecipeId exists
            var recipe = await _context.Recipes.FindAsync(commentDto.RecipeId);
            if (recipe == null)
            {
                return BadRequest(new { message = "Recipe not found" });
            }

            var userId =commentDto.AuthorId;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var comment = new Comment
            {
                Content = commentDto.Content,
                RecipeId = commentDto.RecipeId,
                CreatedAt = DateTime.UtcNow, // Set current time when creating the comment
                AuthorId = user.Id
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Create the response DTO including the author's name
            var responseDto = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                RecipeId = comment.RecipeId,
                AuthorId = comment.AuthorId,
                CreatedAt = comment.CreatedAt,
                AuthorName = user.FirstName + " " + user.LastName // Assuming `AuthorName` is added to CommentDto
            };

            return CreatedAtAction(nameof(GetCommentsForRecipe), new { id = comment.RecipeId }, responseDto);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
