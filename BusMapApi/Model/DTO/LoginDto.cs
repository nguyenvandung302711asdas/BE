using System.ComponentModel.DataAnnotations;

namespace BusMapApi.Model.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Tên Email không được để trống.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public required string Password { get; set; }

    }
}
