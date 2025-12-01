using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Repository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenService _authenService;

        public AuthenticationController(IAuthenService authenService)
        {
            _authenService = authenService;
        }

        [HttpPost("register-send-otp")]
        public async Task<IActionResult> SendOtpSignUp(RegisterRequest request)
        {
            var result = await _authenService.SendEmailConfirmation(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("register-confirm-otp")]
        public async Task<IActionResult> ConfirmOtpSignUp(ConfirmOtpRequest confirmOtpRequest)
        {
            var result = await _authenService.RegisterAsync(confirmOtpRequest);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<AuthenticationResponse> SignIn(AuthenticationRequest request)
        {
            return await _authenService.LoginAsync(request);
        }

        [HttpPost("authentication")]
        public async Task<IActionResult> Authentication([FromBody] string token)
        {
            var reader = await _authenService.AuthenticationAsync(token);
            if (reader == null) return NotFound();

            return Ok(reader);

        }

        [HttpPost("refresh-token")]
        public async Task<RefreshTokenResponse> RefreshToken([FromBody] string token)
        {
            return await _authenService.refreshTokenAsync(token);
        }

        [HttpGet("login-google")]
        public IActionResult inGoogle(string returnUrl = "/api/Authentication/profile")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Redirect("/auth/login-google");
            }

            // Lấy thông tin từ claim của Google
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var avatar = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            var response = await _authenService.LoginWithGoogleAsync(email, name, avatar);

            var html = $@"
                        <html>
                        <head>
                            <meta charset='utf-8'>
                            <title>Đăng nhập</title>
                        </head>
                        <body>
                            <script>
                                if (window.opener) {{
                                    window.opener.postMessage({{
                                        type: 'google-auth-token',
                                        token: '{response.Token}',
                                        refreshToken: '{response.refreshToken}',
                                        iduser: '{response.iduser}'
                                    }}, '*');
                                    window.close();
                                }}
                            </script>
                            <p>Đang xử lý đăng nhập, vui lòng chờ...</p>
                            <p>Access Token {response.Token}</p>
                        </body>
                        </html>
                    ";
            return Content(html, "text/html");
        }

        // Endpoint đăng xuất
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrEmpty(request.refreshToken))
            {
                return BadRequest("Refresh token is required");
            }

            try
            {
                await _authenService.LogoutAsync(request);
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
