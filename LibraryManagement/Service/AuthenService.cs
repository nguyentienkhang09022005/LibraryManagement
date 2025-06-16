using AutoMapper;
using FluentEmail.Core;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Helpers.Interface;
using LibraryManagement.Models;
using LibraryManagement.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Repository
{
    public class AuthenService : IAuthenService
    {
        private readonly LibraryManagermentContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly IFluentEmail _fluentEmail;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _tempOtp;

        private static readonly Guid DefaultTypeReaderId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        public AuthenService(LibraryManagermentContext context, ITokenGenerator tokenGenerator,
            IMapper mapper, IFluentEmail email, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _configuration = configuration; 
            _tempOtp = memoryCache;
            _fluentEmail = email; 
            _context = context;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
        }

        // Hàm tạo id Reader
        public async Task<string> generateNextIdReaderAsync()
        {
            var nextID = await _context.Readers.OrderByDescending(id => id.IdReader).FirstOrDefaultAsync();

            int nextNumber = 1;

            if (nextID != null && nextID.IdReader.StartsWith("rd"))
            {
                string numberPart = nextID.IdReader.Substring(2);
                if (int.TryParse(numberPart, out int parsed))
                {
                    nextNumber = parsed + 1;
                }
            }
            return $"rd{nextNumber:D5}";
        }

        // Hàm đăng nhập
        public async Task<AuthenticationResponse> SignInAsync(AuthenticationRequest request)
        {
            var reader = await _context.Readers.FirstOrDefaultAsync(reader => reader.ReaderUsername == request.username);
            if (reader == null || !BCrypt.Net.BCrypt.Verify(request.password, reader.ReaderPassword))
                throw new Exception("Unauthenticated");

            var _token = _tokenGenerator.GenerateToken(reader);
            var _refreshToken = _tokenGenerator.GenerateRefreshToken(reader);
            return new AuthenticationResponse
            {
                Token = _token,
                refreshToken = _refreshToken, 
                iduser = reader.IdReader.ToString(),
            };
        }

        // Hàm đăng ký
        public async Task<bool> SignUpWithOtpAsync(ConfirmOtp confirmOtp)
        {
            var checkEmail = await _context.Readers.FirstOrDefaultAsync(e => e.Email == confirmOtp.Email);
            if (checkEmail != null)
            {
                throw new Exception("Email existed");
            }

            var newRole = await _context.Roles.FirstOrDefaultAsync(role => role.RoleName == AppRoles.Reader);

            if (!_tempOtp.TryGetValue($"OTP_{confirmOtp.Email}", out dynamic? cacheData)) return false;

            if (cacheData.Otp != confirmOtp.Otp) return false; 

            if (newRole == null) // Nếu role Reader chưa có trong csdl
            {
                newRole = new Role
                {
                    RoleName = AppRoles.Reader,
                    Description = "Reader Role"
                };
                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();
            }

            var reader = new Reader
            {
                IdReader = await generateNextIdReaderAsync(),
                Email = confirmOtp.Email,
                ReaderUsername = confirmOtp.Email,
                ReaderPassword = BCrypt.Net.BCrypt.HashPassword(cacheData.Password),
                IdTypeReader = DefaultTypeReaderId,
                RoleName = newRole.RoleName,
                CreateDate = DateTime.UtcNow,
                CheckLogin = Helper.LoginNormal,
            };
           
            await _context.Readers.AddAsync(reader);
            await _context.SaveChangesAsync();
            return true;
        }

        // Hàm gửi OTP xác thực
        public async Task<bool> SendEmailConfirmation(SignUpModel signup)
        {
            var user = await _context.Readers.FirstOrDefaultAsync(x => x.ReaderUsername == signup.Email);
            if (user != null)
            {
                return false; 
            }
            try
            {
                var otp = new Random().Next(100000, 999999).ToString();

                _tempOtp.Set($"OTP_{signup.Email}", new
                {
                    Otp = otp,
                    Password = signup.Password
                }, TimeSpan.FromMinutes(1));
                await _fluentEmail.To(signup.Email)
                    .SetFrom("noreply@gmail.com")
                    .Subject("Mã OTP xác thực của bạn là:")
                    .Body($"<p>Mã OTP của bạn là: <strong>{otp}</strong> (hiệu lực trong 1 phút).</p>", true)
                    .SendAsync();
                return true;
            }
            catch
            {
                return false;

            }
        }

        // Hàm đăng nhập bằng Access Token
        public async Task<ReaderAuthenticationResponse?> AuthenticationAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return null;

            var tokenHanlder = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]); 

            try
            {
                tokenHanlder.ValidateToken(accessToken, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken  );
                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email)) return null;
                var reader = await _context.Readers.Where(x => x.ReaderUsername == email).Select( a => new ReaderAuthenticationResponse
                {
                    IdReader = a.IdReader, 
                    IdTypeReader = a.IdTypeReader,
                    NameReader = a.NameReader,
                    Sex=a.Sex, 
                    Address = a.Address, 
                    Email = a.Email, 
                    Dob = a.Dob,
                    CreateDate = a.CreateDate, 
                    ReaderUsername = a.ReaderUsername, 
                    RoleName = a.RoleName,
                }).FirstOrDefaultAsync() ;
                return reader;
            }
            catch
            {
                return null; 
            }
        }

        // Hàm refresh Token
        public async Task<RefreshTokenResponse> refreshTokenAsync(string Token)
        {
            var user = await AuthenticationAsync(Token);
           
            if (user == null)
            {
                throw new Exception("Invalid or Expired Token");
            }
             var reader = await _context.Readers.FirstOrDefaultAsync(x => x.IdReader == user.IdReader);
            if ( reader == null)
            {
                throw new Exception("Invalid or Expired Token");
            }

            var accessTokenResponse = _tokenGenerator.GenerateToken(reader);

            return new RefreshTokenResponse
            {
                AccessToken = accessTokenResponse
            };
        }

        public async Task<AuthenticationResponse> LoginWithGoogleAsync(
      string email, string fullname, string avatar, DateTime? dateOfBirth = null)
        {
            // 1. Tìm user trong DB
            var reader = await _context.Readers.FirstOrDefaultAsync(x => x.ReaderUsername == email);

            // 2. Nếu chưa có, tạo mới
            if (reader == null)
            {
                var newRole = await _context.Roles.FirstOrDefaultAsync(role => role.RoleName == AppRoles.Reader);
                if (newRole == null)
                {
                    newRole = new Role
                    {
                        RoleName = AppRoles.Reader,
                        Description = "Reader Role"
                    };
                    _context.Roles.Add(newRole);
                    await _context.SaveChangesAsync();
                }

                reader = new Reader
                {
                    IdReader = await generateNextIdReaderAsync(),
                    NameReader = fullname,
                    Dob = dateOfBirth ?? DateTime.MinValue,
                    ReaderUsername = email,
                    ReaderPassword = string.Empty,
                    IdTypeReader = DefaultTypeReaderId,
                    RoleName = newRole.RoleName,
                    CreateDate = DateTime.UtcNow,
                    Email = email,
                    CheckLogin = Helper.LoginGogle,
                };

                 await _context.Readers.AddAsync(reader);

                // Lưu Avatar nếu bạn muốn
                if (!string.IsNullOrEmpty(avatar))
                {
                    var newAvatar = new Image
                    {
                        IdReader = reader.IdReader,
                        Url = avatar,
                    };
                    _context.Images.Add(newAvatar);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                reader.NameReader = fullname;
                if (dateOfBirth != null) reader.Dob = dateOfBirth.Value;

                // Cập nhật avatar nếu bạn muốn, ví dụ:
                var existAvatar = await _context.Images.FirstOrDefaultAsync(x => x.IdReader == reader.IdReader);
                if (existAvatar == null && !string.IsNullOrEmpty(avatar))
                {
                    _context.Images.Add(new Image { IdReader = reader.IdReader, Url = avatar });
                }
                else if (existAvatar != null && !string.IsNullOrEmpty(avatar))
                {
                    existAvatar.Url = avatar;
                }

                await _context.SaveChangesAsync();
            }

            // 3. Trả về token JWT
            var token = _tokenGenerator.GenerateToken(reader);
            var refreshToken = _tokenGenerator.GenerateRefreshToken(reader);

            return new AuthenticationResponse
            {
                Token = token,
                refreshToken = refreshToken,
                iduser = reader.IdReader.ToString()
            };
        }
    }
}
