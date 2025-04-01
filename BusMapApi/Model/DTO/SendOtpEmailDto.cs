using System.ComponentModel.DataAnnotations;

namespace BusMapApi.Model.DTO
{
    public class SendOtpEmailDto
    {

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public required string Email { get; set; }
    }
}
