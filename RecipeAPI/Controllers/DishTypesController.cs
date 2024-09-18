using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.Data;
using RecipeAPI.Models;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DishTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DishTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishType>>> GetDishTypes()
        {
            var dishTypes = await _context.DishTypes.ToListAsync();
            return Ok(dishTypes);
        }

        // GET: api/DishTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DishType>> GetDishType(int id)
        {
            var dishType = await _context.DishTypes.FindAsync(id);

            if (dishType == null)
            {
                return NotFound();
            }

            return Ok(dishType);
        }

        // POST: api/DishTypes
        [HttpPost]
        public async Task<ActionResult<DishType>> PostDishType(DishTypeDto dishTypeDto)
        {
            var dishType = new DishType
            {
                Name = dishTypeDto.Name
            };

            _context.DishTypes.Add(dishType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDishType), new { id = dishType.Id }, dishType);
        }

        // PUT: api/DishTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDishType(int id, DishTypeDto dishTypeDto)
        {
            if (id != dishTypeDto.Id)
            {
                return BadRequest();
            }

            var dishType = await _context.DishTypes.FindAsync(id);
            if (dishType == null)
            {
                return NotFound();
            }

            dishType.Name = dishTypeDto.Name;

            _context.Entry(dishType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishTypeExists(id))
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

        // DELETE: api/DishTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDishType(int id)
        {
            var dishType = await _context.DishTypes.FindAsync(id);
            if (dishType == null)
            {
                return NotFound();
            }

            _context.DishTypes.Remove(dishType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishTypeExists(int id)
        {
            return _context.DishTypes.Any(e => e.Id == id);
        }
    }
}
