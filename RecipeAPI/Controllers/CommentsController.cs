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
        public async Task<ActionResult<CommentDto>> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Recipe)
                .Where(c => c.Id == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    RecipeId = c.RecipeId,
                    AuthorId = c.AuthorId,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            return Ok(comment);
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

            var comment = new Comment
            {
                Content = commentDto.Content,
                RecipeId = commentDto.RecipeId,
                CreatedAt = commentDto.CreatedAt
            };

            // Set the AuthorId based on the currently authenticated user
            var userId = User.Identity?.Name;
            if (userId != null)
            {
                var user = await _userManager.FindByNameAsync(userId);
                if (user != null)
                {
                    comment.AuthorId = user.Id;
                }
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            commentDto.Id = comment.Id; // Set the ID for the returned DTO

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, commentDto);
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
