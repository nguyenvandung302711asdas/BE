
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Route("api/[controller]")]
[ApiController]
public class MapboxController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
     private readonly string _mapboxToken;

    public MapboxController(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _accessToken = config["Mapbox:AccessToken"];
    }


    // Lay ban dodo

         [HttpGet("tile-layer")]
        public IActionResult GetTileLayer()
        {
            var mapData = new
            {
                tileUrl = $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/tiles/{{z}}/{{x}}/{{y}}?access_token={_accessToken}"
            };

            return Ok(mapData);
        }

     //Ve ban do
    [HttpGet("route")]
    public async Task<IActionResult> GetRoute([FromQuery] string start, [FromQuery] string end)
    {
        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
        {
            return BadRequest("Vui lòng nhập đầy đủ địa chỉ.");
        }

        // 🛠 Chuyển địa chỉ thành tọa độ (Geocoding API)
        var startCoords = await GetCoordinates(start);
        var endCoords = await GetCoordinates(end);

        if (startCoords == null || endCoords == null)
        {
            return BadRequest("Không tìm thấy tọa độ của địa chỉ.");
        }

        // 🛠 Tạo URL để lấy đường đi
        string routeUrl = $"https://api.mapbox.com/directions/v5/mapbox/driving/{startCoords};{endCoords}?geometries=geojson&access_token={_accessToken}";

        var response = await _httpClient.GetAsync(routeUrl);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Lỗi khi lấy dữ liệu đường đi từ Mapbox.");
        }

        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }

    // 📌 Hàm chuyển địa chỉ thành tọa độ
    private async Task<string?> GetCoordinates(string address)
    {
        string encodedAddress = HttpUtility.UrlEncode(address);
        string geocodeUrl = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{encodedAddress}.json?access_token={_accessToken}";

        var response = await _httpClient.GetAsync(geocodeUrl);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        if (!json.RootElement.TryGetProperty("features", out var features) || features.GetArrayLength() == 0)
        {
            return null;
        }

        var coordinates = features[0].GetProperty("geometry").GetProperty("coordinates");
        return $"{coordinates[0]},{coordinates[1]}"; // longitude,latitude
    }

    
  
    [HttpGet("autocomplete")]
    public async Task<IActionResult> GetAddressSuggestions([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query cannot be empty.");

        string url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{query}.json?access_token={_accessToken}&autocomplete=true&country=VN&limit=5";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Failed to fetch data from Mapbox.");

        var data = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(data);

        // Kiểm tra nếu không có địa chỉ nào phù hợp
        if (!json.ContainsKey("features") || json["features"]?.HasValues == false)
            return NotFound("No address suggestions found.");

        // Trả về danh sách địa chỉ gọn gàng hơn
        var results = json["features"]
            .Select(f => new
            {
                Name = f["place_name"]?.ToString(),
                Longitude = f["center"]?[0]?.ToString(),
                Latitude = f["center"]?[1]?.ToString()
            })
            .ToList();

        return Ok(results);
    }

}
