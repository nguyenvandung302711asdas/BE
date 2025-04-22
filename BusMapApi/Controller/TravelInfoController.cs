using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using BusMapApi.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
namespace BusMapApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelInfoController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public TravelInfoController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/TravelInfo
        [HttpPost]
        public async Task<IActionResult> PostTravelInfo([FromBody] TravelInfo travelInfo)
        {
            var user = await _context.Users.FindAsync(travelInfo.UserId);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            _context.TravelInfos.Add(travelInfo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Lưu thông tin di chuyển thành công!",
                travelInfo
            });
        }

        // GET: api/TravelInfo/user/1
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TravelInfo>>> GetTravelInfosByUser(int userId)
        {
            var travelInfos = await _context.TravelInfos
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return Ok(travelInfos);
        }
    }
}