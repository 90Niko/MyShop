using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;
using MyShop.Data.Models;
using MyShop.DTO.ModelsDto;

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

        [HttpGet("getAll")]
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
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(ProductModel model)
        {
            if (model == null)
            {
                return BadRequest(new { Message = "Invalid product data!" });
            }

            if (string.IsNullOrWhiteSpace(model.Category))
            {
                return BadRequest(new { Message = "Category is required!" });
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.Category);

            if (category == null)
            {
                return NotFound(new { Message = $"Category '{model.Category}' does not exist!" });
            }

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Category = category,
                CreatedOn = DateTime.UtcNow

            };

            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Product added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound(new { Message = "Product not found in the database." });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Product deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditProduct(int id, ProductModel model)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.Category);

            if (category == null)
            {
                return NotFound(new { Message = $"Category '{model.Category}' does not exist!" });
            }

            try
            {
                // Validate the input model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid data provided." });
                }

                // Find the existing product in the database
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new { Message = "Product not found in the database." });
                }

                // Update the product properties
                existingProduct.Name = model.Name;
                existingProduct.Price = model.Price;
                existingProduct.Description = model.Description;
                existingProduct.Category = category;
               
                // Save changes to the database
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Product updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

