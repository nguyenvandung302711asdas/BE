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

namespace BusMapApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly HttpClient _httpClient;
        private const string OverpassUrl = "https://overpass-api.de/api/interpreter";
        private const string Query = "[out:json];node[highway=bus_stop](10.5,106.5,11,107);out;";
        private const string ApiUrl = "http://apicms.ebms.vn/businfo/getallroute";


        private readonly IConfiguration _configuration;



        public AccountsController(ApplicationDBContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }
       
        [HttpGet("id")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }
        // PUT: api/Account/5
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDto dto)
        {
            var existingAccount = await _context.Accounts.FindAsync(id);
            if (existingAccount == null)
                return NotFound();

            existingAccount.FullName = dto.FullName;
            existingAccount.Email = dto.Email;
            existingAccount.NumberPhone = dto.NumberPhone;
            existingAccount.Password = dto.Password;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công.");
            }
            catch (DbUpdateException)
            {
                if (_context.Accounts.Any(a => a.Email == dto.Email && a.Id != id))
                    return Conflict("Email đã tồn tại.");

                return StatusCode(500, "Lỗi khi cập nhật.");
            }
        }


        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAddmins()
        {
            return await _context.Admin.ToListAsync();
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

                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == loginDto.Email);

                if (account == null)
                {
                    return BadRequest(new { message = "Tên Email không tồn tại!" });
                }

                // Kiểm tra mật khẩu với BCrypt
                if (loginDto.Password != account.Password)
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
        [HttpGet("get login")]
        public async Task<ActionResult<IEnumerable<Admin>>> GetLogin()
        {
            return await _context.Admin.ToListAsync();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto accountDto)
        {
            try
            {

                bool isEmailExists = await _context.Accounts.AnyAsync(a => a.Email == accountDto.Email);
                bool isEmailAdminExists = await _context.Admin.AnyAsync(a => a.Email == accountDto.Email);


                if (isEmailExists ||isEmailAdminExists )
                    return BadRequest(new { message = "Email đã được sử dụng!" });

                // Mã hóa mật khẩu
                //string hashedPassword = BCrypt.Net.BCrypt.HashPassword();

                var account = new Account
                {
                    FullName = accountDto.Fullname,
                    Email = accountDto.Email,
                    NumberPhone = accountDto.PhoneNumber,
                    Password = accountDto.Password
                };

                _context.Accounts.Add(account);
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
                var getEmail = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Email.ToLower() == request.Email.ToLower());

                //var getEmail = await _context.Account.FirstOrDefaultAsync(a => a.Email == request.Email);
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
     .Where(f => f.Email.ToLower() == request.Email.ToLower())
     .OrderByDescending(f => f.Id)
     .FirstOrDefaultAsync();


                if (otpRecord == null || otpRecord.OTP != request.OTP)
                {
                    return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn!" });
                }

                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email);
                if (account == null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản với email này!" });
                }

                if (request.NewPassword != request.ConfirmPassword)
                {
                    return BadRequest(new { message = "Mật khẩu xác nhận không khớp!" });
                }

                account.Password =(request.NewPassword);

                _context.Accounts.Update(account);
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
