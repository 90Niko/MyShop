using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;
using MyShop.Data.Models;
using MyShop.DTO.ModelsDto;

namespace MyShop.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Order object is null");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    CustomerAddress = order.Address,
                    TotalPrice = order.TotalPrice,
                    Status = "Pending",
                    OrderItems = order.OrderItems.Select(oi => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Price= oi.Price
                    }).ToList()
                };
                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();
                return CreatedAtAction("CreateOrder", new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


    }
}
