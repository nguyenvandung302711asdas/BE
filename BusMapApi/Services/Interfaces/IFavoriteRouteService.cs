using BusMapApi.Model.DTOs;
using BusMapApi.Model.Entities;

namespace BusMapApi.Services.Interfaces
{
    public interface IFavoriteRouteService
    {
        Task<List<FavoriteRoute>> GetFavoriteRoutesAsync(int userId);
        Task<FavoriteRoute> AddFavoriteRouteAsync(int userId, FavoriteRouteDto favoriteRouteDto);
        Task<bool> DeleteFavoriteRouteAsync(int userId, Guid id);
    }
}