using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.IRepository
{
    public interface IAuthenService
    {
        Task<ApiResponse<string>> RegisterAsync(ConfirmOtpRequest confirmOtpRequest);

        public Task<AuthenticationResponse> LoginAsync(AuthenticationRequest request);

        Task<ApiResponse<string>> SendEmailConfirmation(RegisterRequest registerRequest);

        public Task<ReaderAuthenticationResponse?> AuthenticationAsync(string accessToken);


        public Task<RefreshTokenResponse> refreshTokenAsync(string Token);

        public Task<AuthenticationResponse> LoginWithGoogleAsync(string email, string fullname, string avatar, DateTime? dateOfBirth = null);

        public Task LogoutAsync(LogoutRequest request);
    }
}
