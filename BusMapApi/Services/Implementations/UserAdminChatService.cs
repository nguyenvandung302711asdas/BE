using api.Data;
using BusMapApi.Model.Entities;
using BusMapApi.Models.DTOs;
using BusMapApi.Models.Entities;
using BusMapApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace BusMapApi.Services.Implementations
{
    public class UserAdminChatService : IUserAdminChatService
    {
        private readonly ApplicationDBContext _context;

        public UserAdminChatService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<UserAdminChat> CreateChatAsync(UserAdminChatDto chatDto)
        {
            if (chatDto == null || chatDto.UserId <= 0 || chatDto.AdminId <= 0)
            {
                throw new ArgumentException("Dữ liệu không hợp lệ. Vui lòng cung cấp UserId và AdminId hợp lệ.");
            }

            // Kiểm tra xem đã có cuộc trò chuyện giữa user và admin này chưa
            var existingChat = await _context.UserAdminChat
                .FirstOrDefaultAsync(c => c.UserId == chatDto.UserId && c.AdminId == chatDto.AdminId);

            if (existingChat != null)
            {
                return existingChat;
            }

            var newChat = new UserAdminChat
            {
                UserId = chatDto.UserId,
                AdminId = chatDto.AdminId,
                Title = chatDto.Title ?? "Cuộc trò chuyện với Admin",
                CreatedAt = DateTime.Now,
                LastMessageAt = null
            };

            _context.UserAdminChat.Add(newChat);
            await _context.SaveChangesAsync();
            return newChat;
        }

        public async Task<List<UserAdminChat>> GetChatsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId không hợp lệ.");
            }

            return await _context.UserAdminChat
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<UserAdminChat>> GetChatsByAdminIdAsync(int adminId)
        {
            if (adminId <= 0)
            {
                throw new ArgumentException("AdminId không hợp lệ.");
            }

            return await _context.UserAdminChat
                .AsNoTracking()
                .Where(c => c.AdminId == adminId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserAdminMessage> SendMessageAsync(UserAdminMessageDto messageDto)
        {
            if (messageDto == null || messageDto.ChatId == Guid.Empty || string.IsNullOrEmpty(messageDto.SenderRole) || string.IsNullOrEmpty(messageDto.Content))
            {
                throw new ArgumentException("Dữ liệu không hợp lệ. Vui lòng cung cấp ChatId, SenderRole và Content hợp lệ.");
            }

            if (messageDto.SenderRole != "User" && messageDto.SenderRole != "Admin")
            {
                throw new ArgumentException("SenderRole phải là 'User' hoặc 'Admin'.");
            }

            // Kiểm tra xem ChatId có tồn tại không
            var chat = await _context.UserAdminChat.FindAsync(messageDto.ChatId);
            if (chat == null)
            {
                throw new ArgumentException($"Cuộc trò chuyện với ChatId {messageDto.ChatId} không tồn tại.");
            }

            var newMessage = new UserAdminMessage
            {
                ChatId = messageDto.ChatId,
                SenderRole = messageDto.SenderRole,
                Content = messageDto.Content,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.UserAdminMessage.Add(newMessage);

            // Cập nhật LastMessageAt của cuộc trò chuyện
            chat.LastMessageAt = DateTime.Now;
            _context.UserAdminChat.Update(chat);

            await _context.SaveChangesAsync();
            return newMessage;
        }

        public async Task<List<UserAdminMessage>> GetMessagesByChatIdAsync(Guid chatId)
        {
            if (chatId == Guid.Empty)
            {
                throw new ArgumentException("ChatId không hợp lệ.");
            }

            return await _context.UserAdminMessage
                .AsNoTracking()
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid chatId, string readerRole)
        {
            if (string.IsNullOrEmpty(readerRole) || (readerRole != "User" && readerRole != "Admin"))
            {
                throw new ArgumentException("ReaderRole phải là 'User' hoặc 'Admin'.");
            }

            var chatExists = await _context.UserAdminChat.AnyAsync(c => c.Id == chatId);
            if (!chatExists)
            {
                return false;
            }

            // Đánh dấu tất cả tin nhắn thành "đã đọc"
            var userMessages = await _context.UserAdminMessage
                .Where(m => m.ChatId == chatId && m.SenderRole != readerRole)
                .ToListAsync();

            if (!userMessages.Any())
            {
                return false;
            }

            foreach (var userMessage in userMessages)
            {
                if(!userMessage.IsRead)
                    userMessage.IsRead = true;
            }

            _context.UserAdminMessage.UpdateRange(userMessages);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChatAsync(Guid chatId)
        {
            if (chatId == Guid.Empty)
            {
                throw new ArgumentException("ChatId không hợp lệ.");
            }

            var chat = await _context.UserAdminChat.FindAsync(chatId);
            if (chat == null)
            {
                return false;
            }

            _context.UserAdminChat.Remove(chat);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}