using System.ComponentModel.DataAnnotations;

namespace BusMapApi.Model.DTO
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public required string Email { get; set; }
        public required string OTP { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }



    }
}
