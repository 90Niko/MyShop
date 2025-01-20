using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(MyShopDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
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

        [HttpGet("getAll")]
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

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { Message = "Category not found in the database." });
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the category.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] CategoryModel category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var cat = await _context.Categories.FindAsync(id);
                if (cat == null)
                {
                    return NotFound(new { Message = "Category not found in the database." });
                }

                // Update category
                cat.Name = category.Name;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Category updated successfully!", Category = cat });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the category.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
