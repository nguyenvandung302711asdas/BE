using api.Data;
using BusMapApi.Models.DTOs;
using BusMapApi.Model.Entities;
using BusMapApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;


namespace BusMapApi.Services.Implementations;

public class ChatService : IChatService
{
    private readonly ApplicationDBContext _context;
    private static readonly HttpClient client = new HttpClient();

    public ChatService(ApplicationDBContext context)
    {
        _context = context;
    }
    public async Task<Chat> CreateChatAsync(ChatDto chat)
    {
        var newChat = new Chat
        {
            UserId = chat.UserId,
            Title = "Đoạn chat mới",
            CreatedAt = DateTime.Now,
        };
        _context.Chats.Add(newChat);
        await _context.SaveChangesAsync();
        return newChat;
    }

    public async Task<DetailChat> CreateDetailChatAsync(DetailChatDto detail)
    {
        var newDetailChat = new DetailChat
        {
            Role = detail.Role,
            Content = detail.Content,
            ChatId = detail.ChatId,
            CreatedAt = DateTime.Now,
        };

        _context.DetailChats.Add(newDetailChat);

        await _context.SaveChangesAsync();

        return newDetailChat;
    }

    public async Task<List<Chat>> GetChatsByUserIdAsync(int userId)
    {
        try
        {
            return await _context.Chats
                .AsNoTracking()
                .Where(ch => ch.UserId == userId)
                .OrderByDescending(ch => ch.CreatedAt)
                .ToListAsync();
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database error in GetChatHistoryAsync: {dbEx.Message}");
            throw new Exception("Lỗi truy vấn cơ sở dữ liệu.", dbEx);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in GetChatHistoryAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<List<DetailChat>> GetDetailChatByIdAsync(Guid chatId, int page = 1, int pageSize = 20)
    {
        try
        {
            if (chatId == Guid.Empty)
            {
                throw new ArgumentException("ChatId không hợp lệ.", nameof(chatId));
            }

            if (page < 1)
            {
                throw new ArgumentException("Số trang phải lớn hơn hoặc bằng 1.", nameof(page));
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Kích thước trang phải lớn hơn hoặc bằng 1.", nameof(pageSize));
            }

            List<DetailChat> detailChats = await _context.DetailChats
                .Where(dc => dc.ChatId == chatId)
                .OrderByDescending(dc => dc.CreatedAt) // Tin nhắn mới nhất ở đầu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return detailChats;
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database error in GetDetailChatByIdAsync: {dbEx.Message}");
            throw new Exception("Lỗi truy vấn cơ sở dữ liệu.", dbEx);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in GetDetailChatByIdAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteChatAsync(Guid chatId)
    {

        Chat chatHistory = await _context.Chats.FindAsync(chatId);
        if (chatHistory == null) return false;

        _context.Chats.Remove(chatHistory);
        await DeleteDetailChatAsync(chatId);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDetailChatAsync(Guid chatId)
    {
        DetailChat[] detailChats = await _context.DetailChats.Where(dc => dc.ChatId == chatId).ToArrayAsync();
        if (detailChats.Length < 0) return false;

        _context.DetailChats.RemoveRange(detailChats);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DetailChat> PostRequestChat(DetailChatDto request)
    {
        // Thêm vào database DetailChat
        await CreateDetailChatAsync(request);

        var requestBody = new
        {
            model = "qwen/qwen2.5-vl-72b-instruct:free",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new[]
                    {
                        new { type = "text", text = request.Content}
                    }
                }
            }
         };

        string jsonContent = JsonSerializer.Serialize(requestBody);

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions")
        {
            Headers =
        {
            { "Authorization", "Bearer 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" },
            { "Accept", "application/json" }
        },
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response = await client.SendAsync(httpRequest);
        string responseContent = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(responseContent);
        JsonElement root = doc.RootElement;
        string content = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        string createdAt = root.GetProperty("created").ToString();

        var newReponseDetailChat = new DetailChatDto
        {
            Content = content,
            Role = "Ai",
            ChatId = request.ChatId
        };

        var endReponse = await CreateDetailChatAsync(newReponseDetailChat);

        return endReponse;
    }
    public async Task<Chat> UpdateChatTitleAsync(Guid chatId, string newTitle)
    {
        try
        {
            if (chatId == Guid.Empty)
            {
                throw new ArgumentException("ChatId không hợp lệ.", nameof(chatId));
            }

            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException("Tiêu đề không được để trống.", nameof(newTitle));
            }

            var chat = await _context.Chats.FindAsync(chatId);
            if (chat == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy chat với ID {chatId}.");
            }

            chat.Title = newTitle;
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();

            return chat;
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database error in UpdateChatTitleAsync: {dbEx.Message}");
            throw new Exception("Lỗi cập nhật cơ sở dữ liệu.", dbEx);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in UpdateChatTitleAsync: {ex.Message}");
            throw;
        }
    }
}