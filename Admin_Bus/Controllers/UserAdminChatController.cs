using Microsoft.AspNetCore.Mvc;
using Admin_Bus.Data;
using Admin_Bus.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin_Bus.Controllers
{
    [Route("api/user-admin-chats")]
    [ApiController]
    public class UserAdminChatController : ControllerBase
    {
        private readonly ArticleDbContext _context;

        public UserAdminChatController(ArticleDbContext context)
        {
            _context = context;
        }

        // POST: api/user-admin-chats
        [HttpPost]
        public async Task<ActionResult<UserAdminChat>> CreateChat([FromBody] UserAdminChat chat)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (chat.UserId <= 0 || chat.AdminId <= 0)
                    return BadRequest("UserId và AdminId phải lớn hơn 0.");

                var userAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == chat.UserId);
                var adminAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == chat.AdminId);

                if (userAccount == null || adminAccount == null)
                    return BadRequest("UserId hoặc AdminId không tồn tại.");

                if (userAccount.Email.EndsWith("@admin.utc2"))
                    return BadRequest("UserId phải là email của user, không được chứa @admin.utc2.");
                if (!adminAccount.Email.EndsWith("@admin.utc2"))
                    return BadRequest("AdminId phải là email của admin, phải chứa @admin.utc2.");

                var existingChat = await _context.UserAdminChat
                    .FirstOrDefaultAsync(c => c.UserId == chat.UserId && c.AdminId == chat.AdminId);

                if (existingChat != null)
                    return Ok(existingChat);

                chat.Id = Guid.NewGuid();
                chat.Title ??= "Cuộc trò chuyện với Admin";
                chat.CreatedAt = DateTime.Now;
                chat.LastMessageAt = null;

                _context.UserAdminChat.Add(chat);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetChatsByUserId), new { userId = chat.UserId }, chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo cuộc trò chuyện.", error = ex.Message });
            }
        }

        // GET: api/user-admin-chats/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserAdminChat>>> GetChatsByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest("UserId phải lớn hơn 0.");

                var userAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == userId);
                if (userAccount == null)
                    return NotFound($"Không tìm thấy tài khoản với UserId: {userId}.");
                if (userAccount.Email.EndsWith("@admin.utc2"))
                    return BadRequest("UserId này thuộc về admin, không phải user.");

                var chats = await _context.UserAdminChat
                    .AsNoTracking()
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                    .ToListAsync();

                return chats.Count == 0
                    ? NotFound($"Không tìm thấy cuộc trò chuyện nào cho UserId: {userId}.")
                    : Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách cuộc trò chuyện.", error = ex.Message });
            }
        }

        // GET: api/user-admin-chats/admin/{adminId}
        [HttpGet("admin/{adminId}")]
        public async Task<ActionResult<List<UserAdminChat>>> GetChatsByAdminId(int adminId)
        {
            try
            {
                if (adminId <= 0)
                    return BadRequest("AdminId phải lớn hơn 0.");

                var adminAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == adminId);
                if (adminAccount == null)
                    return NotFound($"Không tìm thấy tài khoản với AdminId: {adminId}.");
                if (!adminAccount.Email.EndsWith("@admin.utc2"))
                    return BadRequest("AdminId này không phải admin (email không chứa @admin.utc2).");

                var chats = await _context.UserAdminChat
                    .AsNoTracking()
                    .Where(c => c.AdminId == adminId)
                    .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                    .ToListAsync();

                return chats.Count == 0
                    ? NotFound($"Không tìm thấy cuộc trò chuyện nào cho AdminId: {adminId}.")
                    : Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách cuộc trò chuyện.", error = ex.Message });
            }
        }

        // POST: api/user-admin-chats/message
        [HttpPost("message")]
        public async Task<ActionResult<UserAdminMessage>> SendMessage([FromBody] UserAdminMessage message)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (message.ChatId == Guid.Empty)
                    return BadRequest("ChatId không hợp lệ.");

                if (string.IsNullOrEmpty(message.Content))
                    return BadRequest("Nội dung tin nhắn không được để trống.");

                var chat = await _context.UserAdminChat
                    .FirstOrDefaultAsync(c => c.Id == message.ChatId);

                if (chat == null)
                    return NotFound($"Không tìm thấy cuộc trò chuyện với ChatId: {message.ChatId}.");

                var senderAccount = message.SenderRole == "User"
                    ? await _context.Accounts.FirstOrDefaultAsync(a => a.Id == chat.UserId)
                    : await _context.Accounts.FirstOrDefaultAsync(a => a.Id == chat.AdminId);

                if (senderAccount == null)
                    return BadRequest("Không tìm thấy tài khoản gửi tin nhắn.");

                bool isAdmin = senderAccount.Email.EndsWith("@admin.utc2");
                if (message.SenderRole == "User" && isAdmin)
                    return BadRequest("Tài khoản với email @admin.utc2 không thể gửi với vai trò User.");
                if (message.SenderRole == "Admin" && !isAdmin)
                    return BadRequest("Tài khoản không có email @admin.utc2 không thể gửi với vai trò Admin.");

                message.Id = Guid.NewGuid();
                message.SentAt = DateTime.Now;
                message.IsRead = false;

                chat.LastMessageAt = message.SentAt;

                _context.UserAdminMessage.Add(message);
                _context.UserAdminChat.Update(chat);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMessagesByChatId), new { chatId = message.ChatId }, message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi gửi tin nhắn.", error = ex.Message });
            }
        }

        // GET: api/user-admin-chats/{chatId}/messages
        [HttpGet("{chatId}/messages")]
        public async Task<ActionResult<List<UserAdminMessage>>> GetMessagesByChatId(Guid chatId)
        {
            try
            {
                if (chatId == Guid.Empty)
                    return BadRequest("ChatId không hợp lệ.");

                var messages = await _context.UserAdminMessage
                    .AsNoTracking()
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                return messages.Count == 0
                    ? NotFound($"Không tìm thấy tin nhắn nào cho ChatId: {chatId}.")
                    : Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách tin nhắn.", error = ex.Message });
            }
        }

        // PUT: api/user-admin-chats/chat/{chatId}/mark-read
        [HttpPut("chat/{chatId}/mark-read")]
        public async Task<ActionResult> MarkMessageAsRead(Guid chatId, [FromQuery] string readerRole)
        {
            try
            {
                // Kiểm tra giá trị hợp lệ của readerRole
                if (string.IsNullOrEmpty(readerRole) || (readerRole != "User" && readerRole != "Admin"))
                    return BadRequest("ReaderRole phải là 'User' hoặc 'Admin'.");

                // Lấy thông tin cuộc trò chuyện
                var chat = await _context.UserAdminChat
                    .Include(c => c.User)
                    .Include(c => c.Admin)
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                if (chat == null)
                    return NotFound($"Không tìm thấy cuộc trò chuyện với ChatId: {chatId}.");

                // Kiểm tra tính hợp lệ của Admin và User theo email
                if (chat.Admin == null || chat.User == null)
                    return BadRequest("Cuộc trò chuyện không có thông tin Admin hoặc User.");

                bool readerIsAdmin = readerRole == "Admin";
                bool adminEmailValid = chat.Admin.Email?.EndsWith("@admin.utc2") ?? false;
                bool userEmailValid = chat.User.Email != null && !chat.User.Email.EndsWith("@admin.utc2");

                if (readerIsAdmin && !adminEmailValid)
                    return BadRequest("AdminId của chat không hợp lệ (email không chứa @admin.utc2).");
                if (!readerIsAdmin && !userEmailValid)
                    return BadRequest("UserId của chat không hợp lệ (email chứa @admin.utc2).");

                // Lấy danh sách tin nhắn chưa đọc của đối phương
                var messages = await _context.UserAdminMessage
                    .Where(m => m.ChatId == chatId && m.SenderRole != readerRole && !m.IsRead)
                    .ToListAsync();

                if (!messages.Any())
                    return Ok("Không có tin nhắn nào cần đánh dấu là đã đọc.");

                // Cập nhật trạng thái tin nhắn
                foreach (var message in messages)
                {
                    message.IsRead = true;
                    _context.Entry(message).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã đánh dấu tin nhắn là đã đọc.", totalUpdated = messages.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi đánh dấu tin nhắn là đã đọc.", error = ex.Message });
            }
        }


        // DELETE: api/user-admin-chats/{chatId}
        [HttpDelete("{chatId}")]
        public async Task<ActionResult> DeleteChat(Guid chatId, [FromQuery] string requesterRole = "Admin")
        {
            try
            {
                if (chatId == Guid.Empty)
                    return BadRequest("ChatId không hợp lệ.");

                if (requesterRole != "Admin")
                    return BadRequest("Chỉ Admin mới có quyền xóa cuộc trò chuyện.");

                var chat = await _context.UserAdminChat
                    .Include(c => c.Admin)
                    .FirstOrDefaultAsync(c => c.Id == chatId);

                if (chat == null)
                    return NotFound($"Không tìm thấy cuộc trò chuyện với ChatId: {chatId}.");

                if (chat.Admin == null || !chat.Admin.Email.EndsWith("@admin.utc2"))
                    return Unauthorized("Chỉ tài khoản admin (@admin.utc2) mới có quyền xóa cuộc trò chuyện.");

                _context.UserAdminChat.Remove(chat);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa cuộc trò chuyện.", error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<UserAdminChat>>> GetAllChats()
        {
            try
            {
                var chats = await _context.UserAdminChat
                    .AsNoTracking()
                    .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                    .ToListAsync();
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách tất cả cuộc trò chuyện.", error = ex.Message });
            }
        }
    }
}