using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Data;
using MyShop.Data.Models;
using MyShop.DTO.ModelsDto;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MyShopDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(MyShopDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
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

                // Check if the list is empty
                if (orders.Count == 0)
                {
                    return NotFound("No orders found in the database.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception if needed for debugging
                Console.WriteLine(ex.Message);
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

                if (order.OrderItems == null || order.OrderItems.Count == 0)
                {
                    return BadRequest("Order must contain at least one item.");
                }

                var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();
                var productsInDb = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                if (productsInDb.Count != productIds.Count)
                {
                    return BadRequest("One or more products in the order are not valid.");
                }

                decimal totalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    CustomerAddress = order.Address,
                    TotalPrice = totalPrice,
                    Status = "Pending",
                    OrderItems = order.OrderItems.Select(oi => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                };

                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                // Serialize the newOrder object with ReferenceHandler.Preserve to avoid circular references
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true // Optional: Makes the JSON pretty-printed
                };

                var serializedOrder = JsonSerializer.Serialize(newOrder, options);

                return CreatedAtAction("CreateOrder", new { id = newOrder.Id }, serializedOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
