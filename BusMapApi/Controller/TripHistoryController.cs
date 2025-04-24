using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using BusMapApi.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using api.Data;

namespace BusMapApi.Controllers
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

        // API lấy danh sách tất cả lịch sử chuyến đi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripHistory>>> GetTripHistories()
        {
            var tripHistories = await _context.TripHistory
         .Include(th => th.account) // Nạp thêm thông tin khách hàng
         .Select(th => new
         {
             th.Id,
             AccountName = th.account.FullName, // Lấy tên khách hàng
             th.BusNumber,
             th.StartLocation,
             th.EndLocation,
             th.StartTime,
             th.EndTime,
             th.Duration,
             th.Fare,
             th.PaymentStatus,
             th.CreatedAt
         })
         .ToListAsync();

            return Ok(tripHistories);
        }
    }
}
