using BusMapApi.Model.Entities;
using BusMapApi.Models.DTOs;
using BusMapApi.Models.Entities;
    
namespace BusMapApi.Services.Interfaces
{
    public interface IUserAdminChatService
    {
        Task<UserAdminChat> CreateChatAsync(UserAdminChatDto chatDto);
        Task<List<UserAdminChat>> GetChatsByUserIdAsync(int userId);
        Task<List<UserAdminChat>> GetChatsByAdminIdAsync(int adminId);
        Task<UserAdminMessage> SendMessageAsync(UserAdminMessageDto messageDto);
        Task<List<UserAdminMessage>> GetMessagesByChatIdAsync(Guid chatId);
        Task<bool> MarkMessageAsReadAsync(Guid chatId, string senderRole);
        Task<bool> DeleteChatAsync(Guid chatId);
    }
}