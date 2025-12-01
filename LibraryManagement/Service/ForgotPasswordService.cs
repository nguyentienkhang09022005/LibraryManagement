using LibraryManagement.Dto.Request;
using LibraryManagement.Models;
using LibraryManagement.Service.InterFace;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;

namespace LibraryManagement.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IFluentEmail _fluentEmail;

        public ForgotPasswordService(LibraryManagermentContext context, IFluentEmail fluentEmail)
        {
            _context = context;
            _fluentEmail = fluentEmail;
        }

        // Hàm tạo otp
        public async Task<bool> SendForgotPasswordOtpAsync(string email)
        {
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.Email == email);
            if (reader == null)
                throw new Exception("User not found");

            // Xoá các OTP cũ của reader
            var oldOtps = _context.Otps.Where(x => x.IdReader == reader.IdReader);
            _context.Otps.RemoveRange(oldOtps);

            var otpCode = new Random().Next(100000, 999999);

            var otp = new Otp
            {
                IdOtp = Guid.NewGuid(),
                IdReader = reader.IdReader,
                otp = otpCode,
                expirationTime = DateTime.UtcNow.AddMinutes(1) // Hiệu lực 1 phút
            };

            _context.Otps.Add(otp);
            await _context.SaveChangesAsync();

            await _fluentEmail
                .To(email)
                .Subject("Mã OTP khôi phục mật khẩu")
                .Body($"<p>Mã OTP của bạn là: <strong>{otpCode}</strong>. Hiệu lực trong 1 phút.</p>", true)
                .SendAsync();

            return true;
        }

        // Hàm gửi lại otp
        public async Task<bool> ResendForgotPasswordOtpAsync(string email)
        {
            return await SendForgotPasswordOtpAsync(email);
        }

        // Hàm xác thực otp và email
        public async Task<bool> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request)
        {
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.Email == request.Email);
            if (reader == null)
                throw new Exception("User not found");

            var latestOtp = await _context.Otps
                .Where(r => r.IdReader == reader.IdReader)
                .OrderByDescending(otp => otp.expirationTime)
                .FirstOrDefaultAsync();

            if (latestOtp == null)
                throw new Exception("Không tìm thấy mã OTP");

            if (latestOtp.expirationTime < DateTime.UtcNow)
                throw new Exception("Mã OTP đã hết hạn");

            if (latestOtp.otp.ToString() != request.Otp)
                throw new Exception("Mã OTP không đúng");

            return true;
        }

        // Hàm thay đổi mật khẩu
        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (request.NewPassword != request.RepeatPassword)
                throw new Exception("Mật khẩu không khớp");

            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.Email == request.Email);
            if (reader == null)
                throw new Exception("User not found");

            reader.ReaderPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _context.Readers.Attach(reader);
            _context.Entry(reader).Property(r => r.ReaderPassword).IsModified = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
