using FluentEmail.Core;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Service.InterFace;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagement.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IFluentEmail _email;
        private readonly IMemoryCache _memoryCache;


        public ForgotPasswordService(LibraryManagermentContext context, 
                                     IFluentEmail email, 
                                     IMemoryCache memoryCache)
        {
            _context = context;
            _email = email;
            _memoryCache = memoryCache;
        }

        // Hàm tạo otp
        public async Task<ApiResponse<string>> SendForgotPasswordOtpAsync(EmailRequest request)
        {
            var checkEmail = await _context.Readers.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (checkEmail == null)
            {
                return ApiResponse<string>.FailResponse("Email không tồn tại!", 404);
            }

            try
            {
                var otp = new Random().Next(100000, 999999).ToString();

                var cacheKey = $"OTP_Forgot_Password_{request.Email}";

                var cacheData = new ForgotPasswordCacheData
                {
                    Otp = otp,
                    Email = request.Email,
                };

                _memoryCache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // OTP hết hạn sau 2 phút
                });

                // Gửi OTP đến mail
                var response = await _email
                    .To(request.Email)
                    .Subject("Mã OTP quên mật khẩu")
                    .Tag("otp-forgot-password")
                    .Body($"<p>Mã OTP của bạn là: <strong>{otp}</strong> (hiệu lực trong 2 phút).</p>", true)
                    .SendAsync();

                if (!response.Successful)
                {
                    return ApiResponse<string>.FailResponse("Gửi OTP thất bại! Vui lòng thử lại sau.", 500);
                }

                return ApiResponse<string>.SuccessResponse("OTP đã được gửi đến bạn!", 200, string.Empty);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }

        // Hàm gửi lại otp
        public async Task<ApiResponse<string>> ResendForgotPasswordOtpAsync(EmailRequest request)
        {
            return await SendForgotPasswordOtpAsync(request);
        }

        // Hàm xác thực otp và email
        public async Task<ApiResponse<string>> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request)
        {
            try
            {
                var checkEmail = await _context.Readers.FirstOrDefaultAsync(e => e.Email == request.Email);
                if (checkEmail == null)
                {
                    throw new Exception("Không tìm thấy độc giả!");
                }

                var cacheKey = $"OTP_Forgot_Password_{request.Email}";
                if (!_memoryCache.TryGetValue<ForgotPasswordCacheData>(cacheKey, out var cacheData))
                {
                    return ApiResponse<string>.FailResponse("OTP không hợp lệ hoặc đã hết hạn!", 400);
                }

                if (cacheData.Otp != request.Otp)
                {
                    return ApiResponse<string>.FailResponse("OTP không đúng!", 400);
                }

                return ApiResponse<string>.SuccessResponse("Xác thực OTP thành công!", 200, string.Empty);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }

        // Hàm thay đổi mật khẩu
        public async Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                if (request.NewPassword != request.RepeatPassword)
                    return ApiResponse<string>.FailResponse("Hai mật khẩu không khớp!", 400);

                var reader = await _context.Readers.FirstOrDefaultAsync(r => r.Email == request.Email);
                if (reader == null)
                    return ApiResponse<string>.FailResponse("Không tìm thấy độc giả!", 404);

                reader.ReaderPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                _context.Readers.Attach(reader);
                _context.Entry(reader).Property(r => r.ReaderPassword).IsModified = true;
                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResponse("Thay đổi mật khẩu thành công!", 200, string.Empty);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }
    }
}
