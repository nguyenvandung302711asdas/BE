using api.Data;
using BusMapApi.Model.DTOs;
using BusMapApi.Model.Entities;
using BusMapApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusMapApi.Services
{
    public class GroupChatService : IGroupChatService
    {
        private readonly ApplicationDBContext _context;

        public GroupChatService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<GroupChat> GetPublicGroupChatAsync()
        {
            var publicGroupChat = await _context.GroupChats
                .FirstOrDefaultAsync(gc => gc.IsPublic);

            if (publicGroupChat == null)
            {
                throw new Exception("Không tìm thấy nhóm chat chung.");
            }

            return publicGroupChat;
        }

        public async Task<GroupChatMessage> SendGroupMessageAsync(GroupChatMessageDto messageDto)
        {
            var groupChat = await _context.GroupChats.FindAsync(messageDto.GroupChatId);
            if (groupChat == null)
            {
                throw new Exception("Không tìm thấy nhóm chat.");
            }

            var message = new GroupChatMessage
            {
                Id = Guid.NewGuid(),
                GroupChatId = messageDto.GroupChatId,
                SenderId = messageDto.SenderId,
                Content = messageDto.Content,
                SentAt = DateTime.UtcNow
            };

            _context.GroupChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<GroupChatMessage>> GetGroupMessagesAsync(Guid groupChatId)
        {
            var groupChatExists = await _context.GroupChats.AnyAsync(gc => gc.Id == groupChatId);
            if (!groupChatExists)
            {
                throw new Exception("Không tìm thấy nhóm chat.");
            }

            return await _context.GroupChatMessages
                .Where(m => m.GroupChatId == groupChatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}