namespace LibraryManagement.Dto.Response
{
    public class ForgotPasswordCacheData
    {
        public required string Otp { get; set; }
        public required string Email { get; set; }
    }
}
