using Microsoft.AspNetCore.Mvc;
using BusMapApi.Models.DTOs;
using BusMapApi.Models.Entities;
using BusMapApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusMapApi.Model.Entities;

namespace BusMapApi.Controllers
{
    [Route("api/user-admin-chats")]
    [ApiController]
    public class UserAdminChatController : ControllerBase
    {
        private readonly IUserAdminChatService _chatService;

        public UserAdminChatController(IUserAdminChatService chatService)
        {
            _chatService = chatService;
        }

        // POST: api/user-admin-chats
        [HttpPost]
        public async Task<ActionResult<UserAdminChat>> CreateChat([FromBody] UserAdminChatDto chatDto)
        {
            try
            {
                var newChat = await _chatService.CreateChatAsync(chatDto);
                return CreatedAtAction(nameof(GetChatsByUserId), new { userId = newChat.UserId }, newChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // GET: user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserAdminChat>>> GetChatsByUserId(int userId)
        {
            try
            {
                var chats = await _chatService.GetChatsByUserIdAsync(userId);
                if (chats == null || chats.Count == 0)
                {
                    return NotFound($"Không tìm thấy cuộc trò chuyện nào cho userId: {userId}.");
                }
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // GET: admin/{adminId}
        [HttpGet("admin/{adminId}")]
        public async Task<ActionResult<List<UserAdminChat>>> GetChatsByAdminId(int adminId)
        {
            try
            {
                var chats = await _chatService.GetChatsByAdminIdAsync(adminId);
                if (chats == null || chats.Count == 0)
                {
                    return NotFound($"Không tìm thấy cuộc trò chuyện nào cho adminId: {adminId}.");
                }
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // POST: /message
        [HttpPost("message")]
        public async Task<ActionResult<UserAdminMessage>> SendMessage([FromBody] UserAdminMessageDto messageDto)
        {
            try
            {
                var message = await _chatService.SendMessageAsync(messageDto);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // GET: api/user-admin-chats/{chatId}/messages
        [HttpGet("{chatId}/messages")]
        public async Task<ActionResult<List<UserAdminMessage>>> GetMessagesByChatId(Guid chatId)
        {
            try
            {
                var messages = await _chatService.GetMessagesByChatIdAsync(chatId);
                if (messages == null || messages.Count == 0)
                {
                    return NotFound($"Không tìm thấy tin nhắn nào cho chatId: {chatId}.");
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // PUT: message/{messageId}/read
        [HttpPut("chat/{chatId}/mark-read")]
        public async Task<ActionResult> MarkMessageAsRead(Guid chatId, [FromQuery] string readerRole)
        {
            try
            {
                var result = await _chatService.MarkMessageAsReadAsync(chatId, readerRole);
                if (!result)
                {
                    return NotFound($"Không tìm thấy cuộc trò chuyện với chatId: {chatId}, hoặc không có tin nhắn nào cần đánh dấu.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // DELETE: api/user-admin-chats/{chatId}
        [HttpDelete("{chatId}")]
        public async Task<ActionResult> DeleteChat(Guid chatId)
        {
            try
            {
                var result = await _chatService.DeleteChatAsync(chatId);
                if (!result)
                {
                    return NotFound($"Không tìm thấy cuộc trò chuyện với chatId: {chatId}.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
}