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
                // Find the correct chat session for the recipient
                var chatSession = await _context.ChatSessions
                    .Include(cs => cs.Messages)
                    .FirstOrDefaultAsync(cs => cs.UserEmail == messageDto.Recipient, cancellationToken);

                // If no chat session exists for the recipient, create a new one
                if (chatSession == null)
                {
                    chatSession = new ChatSession
                    {
                        UserEmail = messageDto.Recipient,  // Ensure the session belongs to the recipient
                        Messages = new List<Message>()
                    };
                    await _context.ChatSessions.AddAsync(chatSession, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Create new message
                var message = new Message
                {
                    Sender = messageDto.Sender,
                    Recipient = messageDto.Recipient,  // Now correctly assigning recipient
                    Content = messageDto.Content,
                    Timestamp = DateTime.UtcNow,
                    ChatSessionId = chatSession.Id
                };

                // Add message to the chat session
                chatSession.Messages.Add(message);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new { message = "Message sent successfully", chatSessionId = chatSession.Id });
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

        [HttpGet("getAllChatSessions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetAllChatSessions(CancellationToken cancellationToken)
        {
            try
            {
                var chatSessions = await _context.ChatSessions
                    .Include(cs => cs.Messages)
                    .ToListAsync(cancellationToken);
                if (chatSessions == null)
                {
                    return NotFound("No chat sessions found.");
                }
                var chatSessionDtos = chatSessions
                    .Select(cs => new ChatSessionModel
                    {
                        Id = cs.Id,
                        UserEmail = cs.UserEmail,
                        Messages = cs.Messages.Select(m => new MessageModel
                        {
                            Id = m.Id,
                            Sender = m.Sender,
                            Content = m.Content,
                            Timestamp = m.Timestamp,
                        }).ToList()
                    })
                    .ToList();
                return Ok(chatSessionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat sessions.");
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

