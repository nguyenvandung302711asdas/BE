using System.Text.Json;
using BusMap.Models;
using BusMap.Interfaces;
using static System.Net.WebRequestMethods;

namespace BusMap.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = "bbff8d94a0ca6dbd98b9070430a3054e";
            _baseUrl = "https://api.openweathermap.org/data/2.5/forecast";
        }

        public async Task<WeatherForecastResponse> GetWeatherForecastAsync(string city)
        {
            var url = $"{_baseUrl}?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Không thể lấy dữ liệu thời tiết từ OpenWeatherMap. Status: {response.StatusCode}, Error: {errorContent}");
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("OpenWeatherMap response: " + json); // In log dữ liệu gốc

            var weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return weatherData;
        }
    }
}