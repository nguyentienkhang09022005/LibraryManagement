using LibraryManagement.Dto.Request;
using LibraryManagement.Helpers;

namespace LibraryManagement.Service.InterFace
{
    public interface IForgotPasswordService
    {
        Task<ApiResponse<string>> SendForgotPasswordOtpAsync(EmailRequest request);

        Task<ApiResponse<string>> ResendForgotPasswordOtpAsync(EmailRequest request);

        Task<ApiResponse<string>> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request);

        Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request);
    }
}
