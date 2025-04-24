using api.Data;
using BusMapApi.Model.DTOs;
using BusMapApi.Model.Entities;
using BusMapApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BusMapApi.Services
{
    public class FavoriteRouteService : IFavoriteRouteService
    {
        private readonly ApplicationDBContext _context;

        public FavoriteRouteService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<FavoriteRoute>> GetFavoriteRoutesAsync(int userId)
        {
            return await _context.FavoriteRoutes
                .Where(fr => fr.UserId == userId)
                .ToListAsync();
        }

        public async Task<FavoriteRoute> AddFavoriteRouteAsync(int userId, FavoriteRouteDto favoriteRouteDto)
        {
            // Kiểm tra xem tuyến đã có trong danh sách yêu thích của user chưa
            var existingRoute = await _context.FavoriteRoutes
                .FirstOrDefaultAsync(r => r.UserId == userId && r.RouteNo == favoriteRouteDto.RouteNo);
            if (existingRoute != null)
            {
                throw new Exception($"Tuyến xe buýt {favoriteRouteDto.RouteNo} đã có trong danh sách yêu thích của bạn.");

            }

            var favoriteRoute = new FavoriteRoute
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RouteNo = favoriteRouteDto.RouteNo,
                RouteName = favoriteRouteDto.RouteName // Sử dụng RouteName từ DTO
            };

            _context.FavoriteRoutes.Add(favoriteRoute);
            await _context.SaveChangesAsync();
            return favoriteRoute;
        }

        public async Task<bool> DeleteFavoriteRouteAsync(int userId, Guid id)
        {
            var route = await _context.FavoriteRoutes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (route == null)
            {
                return false;
            }

            _context.FavoriteRoutes.Remove(route);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}