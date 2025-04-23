using BusMap.Models;

namespace BusMap.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherForecastResponse> GetWeatherForecastAsync(string city);
    }
}