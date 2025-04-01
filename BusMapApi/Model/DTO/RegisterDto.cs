using System.ComponentModel.DataAnnotations;

namespace BusMapApi.Model.DTO
{
    public class RegisterDto
    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "Tên người dùng không được để trống.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Tên người dùng không được chứa khoảng trắng.")]
        public required string Fullname { get; set; }
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email phải có đuôi @gmail.com.")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa chữ số.")]
        [MinLength(10, ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại")]
        [MaxLength(11, ErrorMessage = "Vui lòng nhập đúng định dạng số điện thoại")]
        public required string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        public required string Password { get; set; }
      
    }
}
