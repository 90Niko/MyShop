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
    public class MessageController : ControllerBase
    {
        private readonly MyShopDbContext _context;
        private readonly ILogger<MessageController> _logger;

        public MessageController(MyShopDbContext context, ILogger<MessageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMessage([FromBody] MessageModel message, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(message.Sender) || string.IsNullOrWhiteSpace(message.Content))
            {
                return BadRequest("Sender and Content cannot be empty.");
            }

            var newMessage = new Message
            {
                Sender = message.Sender,
                Content = message.Content,
                Timestamp = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(newMessage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetMessages), new { id = newMessage.Id }, newMessage);
        }

        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetMessages(CancellationToken cancellationToken)
        {
            try
            {
                var messages = await _context.Messages
                    .Select(m => new
                    {
                        m.Id,
                        m.Sender,
                        m.Content,
                        m.Timestamp,
                    })
                    .ToListAsync(cancellationToken);

                if (!messages.Any())
                {
                    return NotFound("No messages found in the database.");
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMessage(int id, CancellationToken cancellationToken)
        {
            try
            {
                // Find the message by ID
                var messageToDelete = await _context.Messages.FindAsync(id, cancellationToken);

                if (messageToDelete == null)
                {
                    return NotFound($"Message with ID {id} not found.");
                }

                // Remove the message from the database
                _context.Messages.Remove(messageToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent(); // Return 204 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}

