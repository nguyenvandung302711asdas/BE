using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using api.Data;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using BusMapApi.Model.Entities;
using BusMapApi.Model.DTO;
using System.Net.Mail;
using System.Net;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using System.Xml;
using Google.Apis.Auth;



namespace BusStopAPI.Controllers
{
    [Route("api/busstops")]
    [ApiController]
    public class BusStopController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly HttpClient _httpClient;
        private const string OverpassUrl = "https://overpass-api.de/api/interpreter";
        private const string Query = "[out:json];node[highway=bus_stop](10.5,106.5,11,107);out;";
        private const string ApiUrl = "http://apicms.ebms.vn/businfo/getallroute";


        private readonly IConfiguration _configuration;


        public BusStopController(ApplicationDBContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        // GET: api/busstops
        [HttpGet("TinBusStop")]
        public async Task<ActionResult<IEnumerable<BusStop>>> GetBusStops()
        {
            var busStops = await _context.BusStop.ToListAsync();
            return Ok(busStops);
        }

        // POST: api/busstops/fetch
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchAndSaveBusStops()
        {
            try
            {
                // Kiểm tra xem _httpClient có null không
                if (_httpClient == null)
                    return StatusCode(500, "Lỗi: HttpClient chưa được khởi tạo.");

                string requestUrl = $"{OverpassUrl}?data={Uri.EscapeDataString(Query)}";
                Console.WriteLine($"Đang gọi API: {requestUrl}");

                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Lỗi API: {response.ReasonPhrase}");
                }

                string jsonData = await response.Content.ReadAsStringAsync();
                JObject data;

                try
                {
                    data = JObject.Parse(jsonData);
                }
                catch (Exception jsonEx)
                {
                    return StatusCode(500, $"Lỗi phân tích JSON: {jsonEx.Message}");
                }

                if (data["elements"] == null)
                    return BadRequest("API không trả về dữ liệu hợp lệ.");

                var busStops = data["elements"]
                    .Where(element => element["lat"] != null && element["lon"] != null) // Kiểm tra dữ liệu hợp lệ
                    .Select(element => new BusStop
                    {
                        Name = element["tags"]?["name"]?.ToString() ?? "Chưa có tên",
                        Latitude = element["lat"].ToObject<double>(),
                        Longitude = element["lon"].ToObject<double>(),
                        Address = element["tags"]?["addr:street"]?.ToString() ?? "Không có địa chỉ"
                    })
                    .ToList();

                if (!busStops.Any())
                    return Ok(new { Message = "Không có trạm xe buýt nào trong dữ liệu trả về." });

                // Kiểm tra xem _context có null không
                if (_context == null)
                    return StatusCode(500, "Lỗi: Database Context chưa được khởi tạo.");

                var existingStops = _context.BusStop
                    .Select(b => new { b.Latitude, b.Longitude })
                    .ToList();

                var newBusStops = busStops
                    .Where(bs => !existingStops.Any(e => e.Latitude == bs.Latitude && e.Longitude == bs.Longitude))
                    .ToList();

                if (newBusStops.Any())
                {
                    _context.BusStop.AddRange(newBusStops);
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = $"Đã lưu {newBusStops.Count} trạm xe buýt vào SQL Server!" });
                }

                return Ok(new { Message = "Không có trạm xe buýt mới để thêm." });
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"Lỗi khi gọi API: {httpEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Lỗi khi lưu dữ liệu vào DB: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }


        // GET: api/busstops/directions
        [HttpGet("directions1")]
        public async Task<IActionResult> GetDirections(double startLat, double startLng, double endLat, double endLng)
        {
            try
            {
                string apiKey = _configuration["5b3ce3597851110001cf6248e0883d07791c4641899a71f24f933f5d"];
                if (string.IsNullOrEmpty(apiKey))
                    return BadRequest("API Key không hợp lệ.");

                string orsUrl = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={apiKey}&start={startLng},{startLat}&end={endLng},{endLat}";

                HttpResponseMessage response = await _httpClient.GetAsync(orsUrl);
                if (!response.IsSuccessStatusCode)
                    return BadRequest("Không thể lấy dữ liệu từ OpenRouteService");

                string jsonData = await response.Content.ReadAsStringAsync();
                var routeData = JsonConvert.DeserializeObject<object>(jsonData);
                //List<BusRoute> busRoutes = JsonConvert.DeserializeObject<List<BusRoute>>(jsonData);


                return Ok(routeData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
        [HttpGet("Account")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Account.ToListAsync();
        }
        // Tin
        [HttpPost("Login")]
        public async Task<IActionResult> CheckLogin([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var account = await _context.Account.FirstOrDefaultAsync(a => a.Email == loginDto.Email);

                if (account == null)
                {
                    return BadRequest(new { message = "Tên Email không tồn tại!" });
                }

                // Kiểm tra mật khẩu với BCrypt
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password))
                {
                    return BadRequest(new { message = "Sai mật khẩu!" });
                }

                return Ok(new { message = "Đăng nhập thành công", account });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }
        }
        [HttpPost("Login Admin")]
        public async Task<IActionResult> CheckLoginAdmin([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountAdmin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == loginDto.Email);

                if (accountAdmin == null)
                {
                    return Ok(0); // Trả về 0 nếu không tìm thấy tài khoản
                }
                else if (accountAdmin.Password == loginDto.Password) { }

                return Ok(new { message = "Đăng nhập thành công Admin", accountAdmin });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto accountDto)
        {
            try
            {

                bool isEmailExists = await _context.Account.AnyAsync(a => a.Email == accountDto.Email);

                if (isEmailExists)
                    return BadRequest(new { message = "Email đã được sử dụng!" });

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(accountDto.Password);

                var account = new Account
                {
                    FullName = accountDto.Fullname,
                    Email = accountDto.Email,
                    NumberPhone = accountDto.PhoneNumber,
                    Password = hashedPassword
                };

                _context.Account.Add(account);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đăng ký thành công!", account });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }
        }
        [HttpPost("Send-OTP")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpEmailDto request)
        {
            try
            {
                var getEmail = await _context.Account.FirstOrDefaultAsync(a => a.Email == request.Email);
                if (getEmail == null)
                {
                    return BadRequest(new { message = "Email không tồn tại trong hệ thống!" });
                }

                var random = new Random();
                var otpCode = random.Next(100000, 999999).ToString();

                try
                {
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587, // Cổng SMTP của Gmail
                        Credentials = new NetworkCredential("Trungtin0972@gmail.com", "graa ltfg psdp wywx"),
                        EnableSsl = true, // Bật SSL để bảo mật
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("Trungtin0972@gmail.com"), // Địa chỉ email gửi
                        Subject = "OTP đổi mật khẩu.",
                        Body = "Mã OTP của bạn là: " + otpCode,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add(getEmail.Email);
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Lỗi khi gửi email OTP!", error = ex.Message });
                }

                var newOtp = new ForgotPassword
                {
                    Email = getEmail.Email,
                    OTP = otpCode,
                };
                _context.ForgotPassword.Add(newOtp);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Mã OTP đã được gửi đến email của bạn." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ForgotPasswordDto request)
        {
            try
            {
                var otpRecord = await _context.ForgotPassword
                    .Where(f => f.Email == request.Email)
                    .OrderByDescending(f => f.Id)
                    .FirstOrDefaultAsync();

                if (otpRecord == null || otpRecord.OTP != request.OTP)
                {
                    return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn!" });
                }

                var account = await _context.Account.FirstOrDefaultAsync(a => a.Email == request.Email);
                if (account == null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản với email này!" });
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return BadRequest(new { message = "Mật khẩu xác nhận không khớp!" });
                }

                account.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                _context.Account.Update(account);
                _context.ForgotPassword.Remove(otpRecord); // Xóa OTP sau khi đổi mật khẩu
                await _context.SaveChangesAsync();

                return Ok(new { message = "Mật khẩu đã được thay đổi thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }
        }
    }
}

