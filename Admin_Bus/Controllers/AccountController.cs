using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin_Bus.Models;
using Microsoft.EntityFrameworkCore;
using Admin_Bus.Data;
using System;

namespace Admin_Bus.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly ArticleDbContext _context;

        public AccountsController(ArticleDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> CheckLogin([FromBody] Login loginDto)
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

                // Kiểm tra xem mật khẩu có phải hash BCrypt hợp lệ không
                if (string.IsNullOrEmpty(account.Password))
                {
                    return StatusCode(500, new { message = "Mật khẩu trong hệ thống không hợp lệ. Liên hệ quản trị viên!" });
                }

                // Kiểm tra mật khẩu với BCrypt
                if (loginDto.Password!= account.Password)
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
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register accountDto)
        {
            try
            {

                bool isEmailExists = await _context.Accounts.AnyAsync(a => a.Email == accountDto.Email);

                if (isEmailExists)
                    return BadRequest(new { message = "Email đã được sử dụng!" });

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(accountDto.Password);

                var account = new Account
                {
                    FullName = accountDto.FullName,
                    Email = accountDto.Email,
                    NumberPhone = accountDto.NumberPhone,
                    Password = hashedPassword
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

    }
}
