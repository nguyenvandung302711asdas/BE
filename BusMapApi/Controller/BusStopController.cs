using Microsoft.AspNetCore.Mvc;
using BusMapApi.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using api.Data;
using System.Net.Http;
using System.Text.Json;

namespace BusStopAPI.Controllers
{
    [Route("api/busstops")]
    [ApiController]
    public class BusStopController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly HttpClient _httpClient;
        private const string OverpassUrl = "https://overpass-api.de/api/interpreter";
        private const string Query = "[out:json];node[highway=bus_stop](10.5,106.5,11,107);out;";
        private readonly IConfiguration _configuration;

        public BusStopController(ApplicationDBContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        // GET: api/busstops
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusStop>>> GetBusStops()
        {
            var busStops = await _context.bustop.ToListAsync();
            return Ok(busStops);
        }

       // POST: api/busstops/fetch
[HttpPost("fetch")]
public async Task<IActionResult> FetchAndSaveBusStops()
{
    try
    {
        // Kiểm tra xem _httpClient có null không
        if (_httpClient == null)
            return StatusCode(500, "Lỗi: HttpClient chưa được khởi tạo.");

        string requestUrl = $"{OverpassUrl}?data={Uri.EscapeDataString(Query)}";
        Console.WriteLine($"Đang gọi API: {requestUrl}");

        HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, $"Lỗi API: {response.ReasonPhrase}");
        }

        string jsonData = await response.Content.ReadAsStringAsync();
        JObject data;

        try
        {
            data = JObject.Parse(jsonData);
        }
        catch (Exception jsonEx)
        {
            return StatusCode(500, $"Lỗi phân tích JSON: {jsonEx.Message}");
        }

        if (data["elements"] == null)
            return BadRequest("API không trả về dữ liệu hợp lệ.");

        var busStops = data["elements"]
            .Where(element => element["lat"] != null && element["lon"] != null) // Kiểm tra dữ liệu hợp lệ
            .Select(element => new BusStop
            {
                Name = element["tags"]?["name"]?.ToString() ?? "Chưa có tên",
                Latitude = element["lat"].ToObject<double>(),
                Longitude = element["lon"].ToObject<double>(),
                Address = element["tags"]?["addr:street"]?.ToString() ?? "Không có địa chỉ"
            })
            .ToList();

        if (!busStops.Any())
            return Ok(new { Message = "Không có trạm xe buýt nào trong dữ liệu trả về." });

        // Kiểm tra xem _context có null không
        if (_context == null)
            return StatusCode(500, "Lỗi: Database Context chưa được khởi tạo.");

        var existingStops = _context.bustop
            .Select(b => new { b.Latitude, b.Longitude })
            .ToList();

        var newBusStops = busStops
            .Where(bs => !existingStops.Any(e => e.Latitude == bs.Latitude && e.Longitude == bs.Longitude))
            .ToList();

        if (newBusStops.Any())
        {
            _context.bustop.AddRange(newBusStops);
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Đã lưu {newBusStops.Count} trạm xe buýt vào SQL Server!" });
        }

        return Ok(new { Message = "Không có trạm xe buýt mới để thêm." });
    }
    catch (HttpRequestException httpEx)
    {
        return StatusCode(500, $"Lỗi khi gọi API: {httpEx.Message}");
    }
    catch (DbUpdateException dbEx)
    {
        return StatusCode(500, $"Lỗi khi lưu dữ liệu vào DB: {dbEx.Message}");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Lỗi server: {ex.Message}");
    }
}


        // GET: api/busstops/directions
        [HttpGet("directions")]
        public async Task<IActionResult> GetDirections(double startLat, double startLng, double endLat, double endLng)
        {
            try
            {
                string apiKey = _configuration["5b3ce3597851110001cf6248e0883d07791c4641899a71f24f933f5d"];
                if (string.IsNullOrEmpty(apiKey))
                    return BadRequest("API Key không hợp lệ.");

                string orsUrl = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={apiKey}&start={startLng},{startLat}&end={endLng},{endLat}";

                HttpResponseMessage response = await _httpClient.GetAsync(orsUrl);
                if (!response.IsSuccessStatusCode)
                    return BadRequest("Không thể lấy dữ liệu từ OpenRouteService");

                string jsonData = await response.Content.ReadAsStringAsync();
                var routeData = JsonSerializer.Deserialize<object>(jsonData);
                
                return Ok(routeData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
}
