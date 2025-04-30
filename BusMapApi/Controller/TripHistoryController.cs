using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusMapApi.Model;
using api.Data;

namespace BusMapApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripHistoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public TripHistoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/TripHistory
        [HttpPost]
        public async Task<IActionResult> PostTripHistory([FromBody] TripHistory tripHistory)
        {
            var customer = await _context.Accounts.FindAsync(tripHistory.CustomerId);
            if (customer == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }



            _context.TripHistories.Add(tripHistory);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Lưu lịch sử chuyến đi thành công!",
                tripHistory
            });
        }

        // GET: api/TripHistory/customer/1
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<TripHistory>>> GetTripHistoriesByCustomer(int customerId)
        {
            var tripHistories = await _context.TripHistories
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            return Ok(tripHistories);
        }
        // GET: api/TripHistory
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TripHistory>>> GetTripHistories()
        //{

        //    var tripHistories = await _context.TripHistories.ToListAsync();
        //    return Ok(tripHistories);

        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTripHistories()
        {
            var tripHistories = await _context.TripHistories
                .Include(t => t.Customer)
                .Select(t => new
                {
                    t.Id,
                    t.CustomerId,
                    FullName = t.Customer.FullName, // Lấy fullname từ Account
                    //Email = t.Customer.Email,
                    t.StartLocation,
                    t.EndLocation,
                    t.StartTime,
                    t.EndTime,
                    t.RouteNumber,
                    t.Cost,
                    t.DurationMinutes,
                    t.WalkingDistance,
                    t.BusDistance
                })
                .ToListAsync();

            return Ok(tripHistories);
        }

    }
}