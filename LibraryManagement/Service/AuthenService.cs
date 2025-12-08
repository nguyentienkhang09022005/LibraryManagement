using AutoMapper;
using FluentEmail.Core;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Helpers.Constant;
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
        private readonly IFluentEmail _email;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AuthenService> _logger;



        private const string DefaultAvatar = "https://res.cloudinary.com/df41zs8il/image/upload/v1750223521/default-avatar-icon-of-social-media-user-vector_a3a2de.jpg";

        public AuthenService(LibraryManagermentContext context, 
                             ITokenGenerator tokenGenerator,
                             IFluentEmail email, 
                             IMemoryCache memoryCache, 
                             IConfiguration configuration,
                             ILogger<AuthenService> logger)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _email = email; 
            _context = context;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
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
        public async Task<ApiResponse<AuthenticationResponse>> LoginAsync(AuthenticationRequest request)
        {
            try
            {
                var reader = await _context.Readers.FirstOrDefaultAsync(reader => reader.Email == request.email);
                if (reader == null || !BCrypt.Net.BCrypt.Verify(request.password, reader.ReaderPassword))
                    throw new Exception("Unauthenticated");

                var _token = _tokenGenerator.GenerateToken(reader);
                var _refreshToken = _tokenGenerator.GenerateRefreshToken(reader);

                return ApiResponse<AuthenticationResponse>.SuccessResponse(
                    "Đăng nhập thành công!",
                    200,
                    new AuthenticationResponse
                    {
                        Token = _token,
                        refreshToken = _refreshToken,
                        iduser = reader.IdReader.ToString(),
                    });
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthenticationResponse>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }

        // Hàm đăng ký
        public async Task<ApiResponse<string>> ConfirmOtpRegisterAsync(ConfirmOtpRequest confirmOtpRequest)
        {
            try 
            {
                var checkEmail = await _context.Readers.FirstOrDefaultAsync(e => e.Email == confirmOtpRequest.Email);
                if (checkEmail != null)
                {
                    throw new Exception("Email đã tồn tại!");
                }

                var newRole = await _context.Roles.FirstOrDefaultAsync(role => role.RoleName == ConstantRoles.Reader);
                var newTypeReader = await _context.TypeReaders.FirstOrDefaultAsync(type => type.NameTypeReader == ConstantTypeReader.TypeReader);

                var cacheKey = $"OTP_Register_{confirmOtpRequest.Email}";
                if (!_memoryCache.TryGetValue<RegisterCacheData>(cacheKey, out var cacheData))
                {
                    return ApiResponse<string>.FailResponse("OTP không hợp lệ hoặc đã hết hạn!", 400);
                }

                if (cacheData.Otp != confirmOtpRequest.Otp)
                {
                    return ApiResponse<string>.FailResponse("OTP không đúng!", 400);
                }

                if (newRole == null) 
                {
                    newRole = new Role
                    {
                        RoleName = ConstantRoles.Reader,
                        Description = "Reader Role"
                    };
                    _context.Roles.Add(newRole);
                    await _context.SaveChangesAsync();
                }

                if (newTypeReader == null)
                {
                    newTypeReader = new TypeReader
                    {
                        IdTypeReader = Guid.NewGuid(),
                        NameTypeReader = ConstantTypeReader.TypeReader
                    };
                    _context.TypeReaders.Add(newTypeReader);
                    await _context.SaveChangesAsync();
                }

                var reader = new Reader
                {
                    IdReader = await generateNextIdReaderAsync(),
                    Email = confirmOtpRequest.Email,
                    ReaderPassword = BCrypt.Net.BCrypt.HashPassword(cacheData.Password),
                    IdTypeReader = newTypeReader.IdTypeReader,
                    RoleName = newRole.RoleName,
                    CreateDate = DateTime.UtcNow,
                };

                await _context.Readers.AddAsync(reader);
                await _context.SaveChangesAsync();

                var image = new Image
                {
                    IdReader = reader.IdReader,
                    Url = DefaultAvatar,
                };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();
                return ApiResponse<string>.SuccessResponse("Đăng ký thành công!", 200, string.Empty);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }

        // Hàm gửi OTP xác thực
        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest registerRequest)
        {
            var checkEmail = await _context.Readers.FirstOrDefaultAsync(x => x.Email == registerRequest.Email);
            if (checkEmail != null)
            {
                return ApiResponse<string>.FailResponse("Email đã tồn tại!", 409);
            }

            if (string.IsNullOrWhiteSpace(registerRequest.Email) || !registerRequest.Email.Contains("@"))
            {
                return ApiResponse<string>.FailResponse("Email không hợp lệ!", 400);
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return ApiResponse<string>.FailResponse("Mật khẩu xác nhận không khớp!", 400);
            }
            try
            {
                var otp = new Random().Next(100000, 999999).ToString();

                var cacheKey = $"OTP_Register_{registerRequest.Email}";

                var cacheData = new RegisterCacheData
                {
                    Otp = otp,
                    Email = registerRequest.Email,
                    Password = registerRequest.Password
                };

                _memoryCache.Set(cacheKey, cacheData, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // OTP hết hạn sau 2 phút
                });

                // Gửi OTP đến mail
                var response = await _email
                    .To(registerRequest.Email)
                    .Subject("Mã OTP xác thực đăng ký tài khoản")
                    .Tag("otp-register")
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

        // Hàm đăng nhập bằng Access Token
        public async Task<ApiResponse<ReaderAuthenticationResponse>> AuthenticationAsync(string accessToken)
        {
            var tokenHanlder = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]); 

            try
            {
                tokenHanlder.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email)) return null;
                var reader = await _context.Readers.Where(x => x.Email == email).Select( a => new ReaderAuthenticationResponse
                {
                    IdReader = a.IdReader, 
                    IdTypeReader = a.IdTypeReader,
                    NameReader = a.NameReader,
                    Sex=a.Sex, 
                    Address = a.Address, 
                    Email = a.Email, 
                    Dob = a.Dob,
                    CreateDate = a.CreateDate, 
                    RoleName = a.RoleName,
                    AvatarUrl = a.Images.Select(x=>x.Url).FirstOrDefault() ?? string.Empty,
                }).FirstOrDefaultAsync() ;
                return ApiResponse<ReaderAuthenticationResponse>.SuccessResponse("Đăng nhập thành công!", 200, reader);
            }
            catch (Exception ex)
            {
                return ApiResponse<ReaderAuthenticationResponse>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }

        // Hàm refresh Token
        public async Task<ApiResponse<RefreshTokenResponse>> RefreshTokenAsync(string Token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(Token);

                // Trích xuất jti từ token
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                // Kiểm tra token đã bị thu hồi chưa
                var isRevoked = await _context.InvalidatedTokens.AnyAsync(t => t.IdToken == jti);
                if (isRevoked)
                {
                    throw new Exception("Refresh token has been revoked");
                }

                var verifyToken = await AuthenticationAsync(Token);

                if (verifyToken == null)
                {
                    throw new Exception("Token không hợp lệ hoặc đã hết hạn!");
                }
                var reader = await _context.Readers.FirstOrDefaultAsync(x => x.IdReader == verifyToken.Data.IdReader);
                if (reader == null)
                {
                    throw new Exception("Invalid or Expired Token");
                }

                var accessTokenResponse = _tokenGenerator.GenerateToken(reader);

                return ApiResponse<RefreshTokenResponse>.SuccessResponse(
                    "Refresh Token thành công!", 
                    200,
                    new RefreshTokenResponse
                    {
                        AccessToken = accessTokenResponse
                    });
            }
            catch (Exception ex)
            {
                return ApiResponse<RefreshTokenResponse>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
            
        }

        public async Task<AuthenticationResponse> LoginWithGoogleAsync(string email, 
                                                                       string fullname, 
                                                                       string avatar, 
                                                                       DateTime? dateOfBirth)
        {
            var reader = await _context.Readers.FirstOrDefaultAsync(x => x.Email == email);

            if (reader == null)
            {
                var newRole = await _context.Roles.FirstOrDefaultAsync(role => role.RoleName == ConstantRoles.Reader);
                if (newRole == null)
                {
                    newRole = new Role
                    {
                        RoleName = ConstantRoles.Reader,
                        Description = "Reader Role"
                    };
                    _context.Roles.Add(newRole);
                    await _context.SaveChangesAsync();
                }

                var newTypeReader = await _context.TypeReaders.FirstOrDefaultAsync(type => type.NameTypeReader == ConstantTypeReader.TypeReader);
                if (newTypeReader == null) 
                {
                    newTypeReader = new TypeReader
                    {
                        IdTypeReader = Guid.NewGuid(),
                        NameTypeReader = ConstantTypeReader.TypeReader
                    };
                    _context.TypeReaders.Add(newTypeReader);
                    await _context.SaveChangesAsync();
                }

                reader = new Reader
                {
                    IdReader = await generateNextIdReaderAsync(),
                    NameReader = fullname,
                    Dob = dateOfBirth ?? new DateTime(1970,1,1).ToUniversalTime(),
                    ReaderPassword = string.Empty,
                    Address = string.Empty,
                    IdTypeReader = newTypeReader.IdTypeReader,
                    RoleName = newRole.RoleName,
                    CreateDate = DateTime.UtcNow,
                    Email = email,
                };

                 await _context.Readers.AddAsync(reader);
                if (string.IsNullOrEmpty(avatar)) avatar = DefaultAvatar;
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

            var token = _tokenGenerator.GenerateToken(reader);
            var refreshToken = _tokenGenerator.GenerateRefreshToken(reader);

            return new AuthenticationResponse
            {
                Token = token,
                refreshToken = refreshToken,
                iduser = reader.IdReader.ToString()
            };
        }

        // Hàm logout
        public async Task<ApiResponse<string>> LogoutAsync(LogoutRequest request)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(request.token);

                // Lấy jti từ token
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
                var expiry = jwtToken.ValidTo;

                if (string.IsNullOrEmpty(jti))
                {
                    throw new Exception("Token không chứa jti");
                }

                var invalidatedToken = new InvalidateToken
                {
                    IdToken = jti,
                    ExpiryTime = expiry
                };

                _context.InvalidatedTokens.Add(invalidatedToken);
                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResponse("Đăng xuất thành công!", 200, string.Empty);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.FailResponse("Lỗi hệ thống: " + ex.Message, 500);
            }
        }
    }
}
