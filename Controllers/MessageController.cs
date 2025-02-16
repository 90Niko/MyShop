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
        public async Task<IActionResult> CreateMessage([FromBody] MessageModel messageDto, CancellationToken cancellationToken)
        {
            try
            {
                // Find or create the chat session
                var chatSession = await _context.ChatSessions
                    .Include(cs => cs.Messages)
                    .FirstOrDefaultAsync(cs => cs.UserEmail == messageDto.Sender, cancellationToken);

                if (chatSession == null)
                {
                    chatSession = new ChatSession
                    {
                        UserEmail = messageDto.Sender,
                        Messages = new List<Message>()
                    };
                    await _context.ChatSessions.AddAsync(chatSession, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Create a new message
                var message = new Message
                {
                    Sender = messageDto.Sender,
                    Content = messageDto.Content,
                    Timestamp = DateTime.Now,
                };

                // Add the message to the chat session
                chatSession.Messages.Add(message);
                await _context.SaveChangesAsync(cancellationToken);

                // Map entities to DTOs
                var chatSessionDto = new ChatSessionModel
                {
                    Id = chatSession.Id,
                    UserEmail = chatSession.UserEmail,
                    Messages = chatSession.Messages.Select(m => new MessageModel
                    {
                        Id = m.Id,
                        Sender = m.Sender,
                        Content = m.Content,
                        Timestamp = m.Timestamp,
                    }).ToList()
                };

                // Return the DTOs
                return Ok(new
                {
                    chatSessionId = chatSessionDto.Id,
                    message = chatSessionDto.Messages.LastOrDefault() // Return the latest message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message.");
                return StatusCode(500, new { error = "Internal server error." });
            }
        }

        [HttpGet("getAll/{chatId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetMessagesByChat(int chatId, CancellationToken cancellationToken)
        {
            try
            {
                var chatSession = await _context.ChatSessions
                    .Include(cs => cs.Messages)
                    .FirstOrDefaultAsync(cs => cs.Id == chatId, cancellationToken);

                if (chatSession == null)
                {
                    return NotFound($"No messages found for chat ID {chatId}.");
                }

                var messages = chatSession.Messages
                    .Select(m => new
                    {
                        m.Id,
                        m.Sender,
                        m.Content,
                        m.Timestamp,
                    })
                    .ToList();

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

        [HttpGet("getUserChat/{userEmail}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetMessagesByUser(string userEmail, CancellationToken cancellationToken)
        {
            try
            {
                var chatSession = await _context.ChatSessions
                    .Include(cs => cs.Messages)
                    .FirstOrDefaultAsync(cs => cs.UserEmail == userEmail, cancellationToken);

                if (chatSession == null)
                {
                    return NotFound($"No chat session found for user {userEmail}.");
                }

                var messages = chatSession.Messages
                    .Select(m => new
                    {
                        m.Id,
                        m.Sender,
                        m.Content,
                        m.Timestamp,
                    })
                    .ToList();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}

