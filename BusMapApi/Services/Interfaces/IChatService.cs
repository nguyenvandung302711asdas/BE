using BusMapApi.Models.DTOs;
using BusMapApi.Model.Entities;

namespace BusMapApi.Services.Interfaces
{
    public interface IChatService
    {
        Task<Chat> CreateChatAsync(ChatDto chat);
        Task<List<Chat>> GetChatsByUserIdAsync(int userId);
        Task<List<DetailChat>> GetDetailChatByIdAsync(Guid chatId, int page, int pageSize);
        Task<bool> DeleteChatAsync(Guid chatId);
        Task<DetailChat> PostRequestChat(DetailChatDto request);
        Task<Chat> UpdateChatTitleAsync(Guid chatId, string newTitle); // Thêm phương thức cập nhật tiêu đề
    }
}