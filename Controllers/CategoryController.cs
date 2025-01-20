using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;
using MyShop.Data.Models;
using MyShop.DTO.ModelsDto;

namespace MyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyShopDbContext _context;
        public CategoryController(MyShopDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryModel category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var cat = new Category
                {
                    Name = category.Name
                };
                if (await _context.Categories.AnyAsync(c => c.Name == cat.Name) || cat == null)
                {
                    return BadRequest(new
                    {
                        Message = cat == null ? "Category cannot be null!" : "Category already exists!"
                    });
                }

                await _context.Categories.AddAsync(cat);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Category added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new
                    {
                        c.Id,
                        c.Name
                    })
                    .ToListAsync();
                if (categories == null || categories.Count == 0)
                {
                    return NotFound("No categories found in the database.");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { Message = "Category not found in the database." });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Category deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception (add your logger implementation here)
                //_logger.LogError(ex, "Error occurred while deleting category with ID {Id}", id);

                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
