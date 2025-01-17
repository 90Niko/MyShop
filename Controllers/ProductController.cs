using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;
using MyShop.Data.Models;

namespace MyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyShopDbContext _context;

        public ProductController(MyShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category) // Include Category data
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Price,
                        p.CreatedOn,
                        p.Description,
                        CategoryName = p.Category != null ? p.Category.Name : "No Category"
                    })
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return NotFound("No products found in the database.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

