using BusMapApi.Model.DTOs;
using BusMapApi.Model.Entities;

namespace BusMapApi.Services.Interfaces
{
    public interface IGroupChatService
    {
        Task<GroupChat> GetPublicGroupChatAsync();
        Task<GroupChatMessage> SendGroupMessageAsync(GroupChatMessageDto messageDto);
        Task<List<GroupChatMessage>> GetGroupMessagesAsync(Guid groupChatId);
    }
}
