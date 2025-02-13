using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;

namespace MyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MyShopDbContext _context;
        private readonly IWebHostEnvironment _env;

        public OrderController(MyShopDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderDate,
                        o.OrderItems,
                        o.TotalPrice,
                        o.CustomerName,
                        o.CustomerAddress,
                        o.CustomerEmail,
                        o.CustomerPhone,
                        o.Status
                    })
                    .ToListAsync();
                if (orders == null || orders.Count == 0)
                {
                    return NotFound("No orders found in the database.");
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
