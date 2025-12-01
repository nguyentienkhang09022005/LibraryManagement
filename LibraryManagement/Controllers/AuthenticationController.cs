using LibraryManagement.Dto.Request;
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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenService.RegisterAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("register-confirm-otp")]
        public async Task<IActionResult> ConfirmOtpRegister([FromBody] ConfirmOtpRequest confirmOtpRequest)
        {
            var result = await _authenService.ConfirmOtpRegisterAsync(confirmOtpRequest);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] AuthenticationRequest request)
        {
            var result = await _authenService.LoginAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("authentication")]
        public async Task<IActionResult> Authentication([FromBody] IntrospectRequest request)
        {
            var result = await _authenService.AuthenticationAsync(request.token);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] IntrospectRequest request)
        {
            var result = await _authenService.RefreshTokenAsync(request.token);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var result = await _authenService.LogoutAsync(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}
