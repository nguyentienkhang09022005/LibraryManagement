using LibraryManagement.Dto.Request;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordController(IForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
        }

        // Endpoint gửi otp
        [HttpPost("forgot-password-send-otp")]
        public async Task<IActionResult> sendOTP([FromBody] EmailRequest request)
        {
            var result = await _forgotPasswordService.SendForgotPasswordOtpAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint gửi lại otp
        [HttpPost("resend-otp")]
        public async Task<IActionResult> resendOTP([FromBody] EmailRequest request)
        {
            var result = await _forgotPasswordService.ResendForgotPasswordOtpAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint xác thực otp
        [HttpPost("forgot-password-confirm-otp")]
        public async Task<IActionResult> verifyOTP([FromBody] VerifyOtpRequest request)
        {
            var result = await _forgotPasswordService.VerifyForgotPasswordOtpAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        // Endpoint thay đổi mật khẩu
        [HttpPost("change-password")]
        public async Task<IActionResult> changePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _forgotPasswordService.ChangePasswordAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
