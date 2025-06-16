using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Repository.IRepository
{
    public interface IAuthenService
    {
        // Interface đăng ký
        public Task<bool> SignUpWithOtpAsync(ConfirmOtp confirmOtp);

        // Interface đăng nhập
        public Task<AuthenticationResponse> SignInAsync(AuthenticationRequest request);

        public Task<bool> SendEmailConfirmation(SignUpModel signup);

        public Task<ReaderAuthenticationResponse?> AuthenticationAsync(string accessToken);


        public Task<RefreshTokenResponse> refreshTokenAsync(string Token);

        public Task<AuthenticationResponse> LoginWithGoogleAsync(
      string email, string fullname, string avatar, DateTime? dateOfBirth = null);
    }
}
