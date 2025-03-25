using BusMapApi.Model.DTOs;
using BusMapApi.Model.Entities;
using BusMapApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusMapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteRouteController : ControllerBase
    {
        private readonly IFavoriteRouteService _favoriteRouteService;

        public FavoriteRouteController(IFavoriteRouteService favoriteRouteService)
        {
            _favoriteRouteService = favoriteRouteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FavoriteRoute>>> GetFavoriteRoutes([FromQuery] int userId)
        {
            try
            {
                // Kiểm tra userId hợp lệ
                if (userId <= 0)
                {
                    return BadRequest(new { message = "userId phải lớn hơn 0." });
                }

                var favoriteRoutes = await _favoriteRouteService.GetFavoriteRoutesAsync(userId);
                return Ok(favoriteRoutes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<FavoriteRoute>> AddFavoriteRoute([FromQuery] int userId, [FromBody] FavoriteRouteDto favoriteRouteDto)
        {
            try
            {
                // Kiểm tra userId hợp lệ
                if (userId <= 0)
                {
                    return BadRequest(new { message = "userId phải lớn hơn 0." });
                }

                var favoriteRoute = await _favoriteRouteService.AddFavoriteRouteAsync(userId, favoriteRouteDto);
                return CreatedAtAction(nameof(GetFavoriteRoutes), new { userId }, favoriteRoute);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFavoriteRoute(Guid id, [FromQuery] int userId)
        {
            try
            {
                // Kiểm tra userId hợp lệ
                if (userId <= 0)
                {
                    return BadRequest(new { message = "userId phải lớn hơn 0." });
                }

                var result = await _favoriteRouteService.DeleteFavoriteRouteAsync(userId, id);
                if (!result)
                {
                    return NotFound(new { message = $"Không tìm thấy tuyến xe buýt yêu thích với Id: {id}." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}