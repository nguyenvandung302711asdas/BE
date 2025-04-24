using Microsoft.AspNetCore.Mvc;
using BusMap.Models;
using BusMap.Interfaces;

namespace BusMap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("forecast")]
        public async Task<ActionResult<WeatherForecastResponse>> GetWeatherForecast()
        {
            try
            {
                var forecast = await _weatherService.GetWeatherForecastAsync("Ho Chi Minh");
                return Ok(forecast);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi server", message = ex.Message });
            }
        }
    }
}