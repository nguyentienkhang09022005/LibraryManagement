using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;

namespace LibraryManagement.Repository.IRepository
{
    public interface IAuthenService
    {
        Task<ApiResponse<string>> ConfirmOtpRegisterAsync(ConfirmOtpRequest confirmOtpRequest);

        Task<ApiResponse<AuthenticationResponse>> LoginAsync(AuthenticationRequest request);

        Task<ApiResponse<string>> RegisterAsync(RegisterRequest registerRequest);

        Task<ApiResponse<ReaderAuthenticationResponse>> AuthenticationAsync(string accessToken);

        Task<ApiResponse<RefreshTokenResponse>> RefreshTokenAsync(string Token);

        public Task<AuthenticationResponse> LoginWithGoogleAsync(string email, string fullname, string avatar, DateTime? dateOfBirth = null);

        Task<ApiResponse<string>> LogoutAsync(LogoutRequest request);
    }
}
