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
        private readonly IWebHostEnvironment _env;

        public ProductController(MyShopDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                        p.ImagePath,
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
        public async Task<IActionResult> CreateProduct([FromForm] ProductModel createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure wwwroot exists
            string uploadsFolder = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(uploadsFolder, "images");

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            // Handle image upload
            string imagePath = null;
            if (createProductDto.ImageFile != null && createProductDto.ImageFile.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + createProductDto.ImageFile.FileName;
                var filePath = Path.Combine(imagesFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await createProductDto.ImageFile.CopyToAsync(fileStream);
                }

                // ✅ Use full URL for the image
                imagePath = $"{Request.Scheme}://{Request.Host}/images/{uniqueFileName}";
            }

            // Create the product
            var product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Description = createProductDto.Description,
                CategoryId = createProductDto.CategoryId,
                CreatedOn = DateTime.UtcNow,
                ImagePath = imagePath
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
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

                // Validate if the category exists
                var category = await _context.Categories.FindAsync(model.CategoryId);
                if (category == null)
                {
                    return NotFound(new { Message = $"Category with ID '{model.CategoryId}' does not exist!" });
                }

                // Update the product properties
                existingProduct.Name = model.Name;
                existingProduct.Price = model.Price;
                existingProduct.Description = model.Description;
                existingProduct.CategoryId = model.CategoryId; // Assuming there is a CategoryId field

                // Save changes to the database
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Product updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}

