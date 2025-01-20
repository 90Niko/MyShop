using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

                await _context.Categories.AddAsync(cat);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Category added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
