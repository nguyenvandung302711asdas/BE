namespace BusMapApi.Model.Entities
{
    public class ForgotPassword
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string OTP { get; set; }
    }
}
