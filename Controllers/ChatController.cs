﻿using Microsoft.AspNetCore.Mvc;
using MyShop.Data;
using MyShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MyShop.DTO.ModelsDto;
using NuGet.Protocol.Plugins;
using Message = MyShop.Data.Models.Message;

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

        [HttpPost("sendByCurrentChatSesions")]
        public async Task<ActionResult> SendMessageByCurrentChatSesions([FromBody] SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserEmail) || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("User email and message cannot be empty.");
            }

            var chatSession = await _context.ChatSessions
                .Where(cs => cs.Id == request.ChatSessionId)
                .FirstOrDefaultAsync();

            if (chatSession == null)
            {
                return NotFound("Chat session not found.");
            }

            var newMessage = new Message
            {
                Sender = request.UserEmail,
                Content = request.Message,
                IsRead = false,
                Timestamp = DateTime.Now,
                ChatSessionId = chatSession.Id
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("getAllChatSessions")]
        public async Task<ActionResult<IEnumerable<ChatSession>>> GetAllChatSessions()
        {
            var chatSessions = await _context.ChatSessions
                .Include(cs => cs.Messages)
                .Select(cs => new
                {
                    cs.Id,
                    cs.UserEmail,
                    cs.CreatedAt,
                    cs.Messages,


                })
                .ToListAsync();

            if (chatSessions == null || chatSessions.Count == 0)
            {
                return NotFound("No chat sessions found.");
            }

            return Ok(chatSessions);
        }

        [HttpGet("unReadMessage")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUnReadMessages()
        {
            var messages = await _context.Messages
                .Where(m => m.IsRead == false && m.Sender != "Admin")
                .ToListAsync();

            // If no messages are found, return 200 OK with an empty list and a message
            return Ok(new { unreadCount = messages.Count, messages = messages.Count > 0 ? messages : new List<Message>(), message = messages.Count == 0 ? "No unread messages found." : null });
        }


        [HttpGet("adminMarkAsRead/{userEmail}")]
        public async Task<ActionResult> AdminMarkAsRead(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email cannot be empty.");
            }
            // Get the current chat session for the provided user email
            var chatSession = await _context.ChatSessions
                .Include(cs => cs.Messages)
                .Where(cs => cs.UserEmail == userEmail)
                .FirstOrDefaultAsync();
            if (chatSession == null)
            {
                return NotFound("Chat session not found.");
            }
            // Mark all messages as read
            foreach (var message in chatSession.Messages.Where(m => m.Sender != "Admin" && m.IsRead == false))
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("myChatSession/{userEmail}")]
        public async Task<ActionResult<ChatSession>> GetMyChatSession(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email cannot be empty.");
            }

            // Fetching the chat session with messages
            var chatSession = await _context.ChatSessions
                .Include(cs => cs.Messages)
                .Where(cs => cs.UserEmail == userEmail)
                .Select(cs => new ChatSession
                {
                    Id = cs.Id,
                    UserEmail = cs.UserEmail,
                    CreatedAt = cs.CreatedAt,
                    Messages = cs.Messages.Select(m => new Message
                    {
                        Id = m.Id,
                        Sender = m.Sender,
                        Content = m.Content,
                        Timestamp = m.Timestamp
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            // Return 200 OK with an empty session or a message when no chat session is found
            if (chatSession == null)
            {
                return Ok(new { message = "No chat session found for this user.", chatSession = new ChatSession() });
            }

            return Ok(chatSession);
        }

        [HttpGet("userUnRead/{userEmail}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUserInReadMessages(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email cannot be empty.");
            }
            var messages = await _context.Messages
                .Where(m => m.IsRead == false && m.Sender != "Admin" && m.ChatSession.UserEmail == userEmail)
                .ToListAsync();
            // If no messages are found, return 200 OK with an empty list and a message
            return Ok(new { unreadCount = messages.Count, messages = messages.Count > 0 ? messages : new List<Message>(), message = messages.Count == 0 ? "No unread messages found." : null });
        }

        [HttpGet("adminUnRead")]

        public async Task<ActionResult<IEnumerable<Message>>> GetAdminUnReadMessages()
        {
            var messages = await _context.Messages
                .Where(m => m.IsRead == false && m.Sender == "Admin")
                .ToListAsync();
            // If no messages are found, return 200 OK with an empty list and a message
            return Ok(new { unreadCount = messages.Count, messages = messages.Count > 0 ? messages : new List<Message>(), message = messages.Count == 0 ? "No unread messages found." : null });
        }

        [HttpGet("anyIsUnread")]
        public async Task<ActionResult> AnyIsUnread(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email cannot be empty.");
            }

            // If the user is an admin, skip checking chat sessions
            if (userEmail.EndsWith("@myshop.com")) // or check for user role
            {
                return Ok(false); // Admins don't have unread messages
            }

            // Get the current chat session for the provided user email
            var chatSession = await _context.ChatSessions
                .Include(cs => cs.Messages)
                .Where(cs => cs.UserEmail == userEmail)
                .FirstOrDefaultAsync();

            if (chatSession == null)
            {
                return NotFound("Chat session not found.");
            }

            // Check if there are any unread messages in the current chat session
            var anyIsUnread = chatSession.Messages.Any(m => m.IsRead == false && m.Sender == "Admin");
            return Ok(anyIsUnread);
        }

        [HttpGet("markAsRead")]
        public async Task<ActionResult> MarkAsRead(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email cannot be empty.");
            }

            // Get the current chat session for the provided user email
            var chatSession = await _context.ChatSessions
                .Include(cs => cs.Messages)
                .Where(cs => cs.UserEmail == userEmail)
                .FirstOrDefaultAsync();

            // If no chat session is found, return a 200 OK response with a message
            if (chatSession == null)
            {
                return Ok(new { message = "No chat session found for this user." });
            }

            // Mark all messages sent by Admin as read
            foreach (var message in chatSession.Messages.Where(m => m.Sender == "Admin"))
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Messages marked as read." });
        }


    }
}
