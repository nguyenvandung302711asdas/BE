using BusMapApi.Model.DTOs;
using BusMapApi.Model.Entities;
using BusMapApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusMapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupChatService _groupChatService;

        public GroupChatController(IGroupChatService groupChatService)
        {
            _groupChatService = groupChatService;
        }

        [HttpGet("public")]
        public async Task<ActionResult<GroupChat>> GetPublicGroupChat()
        {
            try
            {
                var groupChat = await _groupChatService.GetPublicGroupChatAsync();
                return Ok(groupChat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPost("message")]
        public async Task<ActionResult<GroupChatMessage>> SendGroupMessage([FromBody] GroupChatMessageDto messageDto)
        {
            try
            {
                var message = await _groupChatService.SendGroupMessageAsync(messageDto);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("messages/{groupChatId}")]
        public async Task<ActionResult<List<GroupChatMessage>>> GetGroupMessages(Guid groupChatId)
        {
            try
            {
                var messages = await _groupChatService.GetGroupMessagesAsync(groupChatId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
}