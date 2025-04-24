    using Microsoft.AspNetCore.Mvc;
    using BusMapApi.Models.DTOs;
    using BusMapApi.Model.Entities;
    using BusMapApi.Services.Interfaces;

    namespace BusMapApi.Controllers
    {
        [Route("api/chats")]
        [ApiController]
        public class ChatController : ControllerBase
        {
            private readonly IChatService _chatService;

            public ChatController(IChatService chatService)
            {
                _chatService = chatService;
            }

            // POST: api/chats
            [HttpPost]
            public async Task<ActionResult<Chat>> CreateChat([FromBody] ChatDto chatDto)
            {
                try
                {
                    if (chatDto == null || chatDto.UserId <= 0)
                    {
                        return BadRequest("Dữ liệu không hợp lệ. Vui lòng cung cấp UserId hợp lệ.");
                    }

                    var newChat = await _chatService.CreateChatAsync(chatDto);
                    return CreatedAtAction(nameof(GetChatsByUserId), new { userId = newChat.UserId }, newChat);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi server: {ex.Message}");
                }
            }

            // GET: api/chats/user/{userId}
            [HttpGet("user/{userId}")]
            public async Task<ActionResult<List<Chat>>> GetChatsByUserId(int userId)
            {
                try
                {
                    if (userId <= 0)
                    {
                        return BadRequest("UserId không hợp lệ.");
                    }

                    var chats = await _chatService.GetChatsByUserIdAsync(userId);
                    if (chats == null || chats.Count == 0)
                    {
                        return NotFound($"Không tìm thấy chat nào cho userId: {userId}.");
                    }

                    return Ok(chats);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi server: {ex.Message}");
                }
            }

            // GET: api/chats/{chatId}/details
            [HttpGet("{chatId}/details")]
            public async Task<ActionResult<IEnumerable<DetailChat>>> GetDetailChatById(Guid chatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            {
                try
                {
                    if (chatId == Guid.Empty)
                    {
                        return BadRequest("ChatId không hợp lệ.");
                    }

                    var detailChats = await _chatService.GetDetailChatByIdAsync(chatId, page, pageSize);
                    return Ok(detailChats);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi server: {ex.Message} - Inner: {ex.InnerException?.Message ?? "Không có inner exception"}");
                }
            }

            // DELETE: api/chats/{chatId}
            [HttpDelete("{chatId}")]
            public async Task<ActionResult> DeleteChat(Guid chatId)
            {
                try
                {
                    if (chatId == Guid.Empty)
                    {
                        return BadRequest("ChatId không hợp lệ.");
                    }

                    var result = await _chatService.DeleteChatAsync(chatId);
                    if (!result)
                    {
                        return NotFound($"Không tìm thấy chat với chatId: {chatId}.");
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi server: {ex.Message}");
                }
            }

            // POST: api/chats/request
            [HttpPost("request")]
            public async Task<ActionResult<DetailChat>> PostRequestChat([FromBody] DetailChatDto request)
            {
                try
                {
                    if (request == null || string.IsNullOrEmpty(request.Content) || request.ChatId == Guid.Empty)
                    {
                        return BadRequest("Dữ liệu không hợp lệ. Vui lòng cung cấp Content và ChatId hợp lệ.");
                    }

                    var response = await _chatService.PostRequestChat(request);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi server: {ex.Message}");
                }
            }

            [HttpPut("{chatId}")]
            public async Task<ActionResult<Chat>> UpdateChatTitle(Guid chatId, [FromBody] UpdateChatTitleDto dto)
            {
                try
                {
                    var updatedChat = await _chatService.UpdateChatTitleAsync(chatId, dto.Title);
                    return Ok(updatedChat);
                }
                catch (KeyNotFoundException ex)
                {
                    return NotFound(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            public class UpdateChatTitleDto
            {
                public string Title { get; set; }
            }
        }
    }