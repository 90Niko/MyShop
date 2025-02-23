using Microsoft.AspNetCore.Mvc;
using MyShop.Data;
using MyShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly MyShopDbContext _context;

        public ChatController(MyShopDbContext context)
        {
            _context = context;
        }

        [HttpGet("send")]
        public async Task<ActionResult> SendMessage(string userEmail, string message)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("User email and message cannot be empty.");
            }

            var chatSession = await _context.ChatSessions
                .Where(cs => cs.UserEmail == userEmail)
                .FirstOrDefaultAsync();
            if (chatSession == null)
            {
                chatSession = new ChatSession
                {
                    UserEmail = userEmail,
                    CreatedAt = DateTime.Now
                };
                _context.ChatSessions.Add(chatSession);
                await _context.SaveChangesAsync();
            }
            var newMessage = new Message
            {
                Sender = userEmail,
                Content = message,
                IsRead = false,
                Timestamp = DateTime.Now,
                ChatSessionId = chatSession.Id
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
