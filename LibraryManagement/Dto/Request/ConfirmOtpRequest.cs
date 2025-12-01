using Microsoft.IdentityModel.Abstractions;

namespace LibraryManagement.Dto.Request
{
    public class ConfirmOtpRequest
    {
        public string Email { get; set; } = null!;

        public string Otp { get; set; } = null!;
    }
}
