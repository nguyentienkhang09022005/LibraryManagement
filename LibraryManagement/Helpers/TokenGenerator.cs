using LibraryManagement.Data;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers.Interface;
using LibraryManagement.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Helpers
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly LibraryManagermentContext _context;

        public TokenGenerator(IConfiguration config, LibraryManagermentContext context)
        {
            _config = config;
            _context = context;
        }

        // Hàm lấy danh sách permssion theo role
        private List<string> GetPermissionsByRole(string roleName)
        {
            return _context.RolePermissions
                .Where(rp => rp.RoleName == roleName)
                .Select(rp => rp.PermissionName)
                .ToList();
        }

        public string GenerateToken(Reader reader)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, reader.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, reader.RoleName.ToString()),
                new Claim(ClaimTypes.NameIdentifier, reader.IdReader.ToString()),

            };

            var scopeList = new List<string>();

            if (!string.IsNullOrEmpty(reader.RoleName))
            {
                
                scopeList.Add(reader.RoleName); // Thêm Role vào danh sách scope

                var permissions = GetPermissionsByRole(reader.RoleName); // Lấy permission từ RolePermission

                scopeList.AddRange(permissions);
            }
            claims.Add(new Claim("scope", string.Join(" ", scopeList)));

            // Khóa để ký Token
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]!));

            // Tạo Access Token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
                );

            var tokenHandle = new JwtSecurityTokenHandler();
            var accessTokenString = tokenHandle.WriteToken(token);
            return accessTokenString;
        }


        public string GenerateRefreshToken(Reader reader)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, reader.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (!string.IsNullOrEmpty(reader.RoleName))
            {
               

                // Nếu bạn vẫn muốn thêm scope thì giữ lại
                var permissions = GetPermissionsByRole(reader.RoleName);
                var scopeList = new List<string> { reader.RoleName };
                scopeList.AddRange(permissions);

                claims.Add(new Claim("scope", string.Join(" ", scopeList))); // Optional
            }
            // Khóa để ký Token
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]!));

            // Tạo Refresh Token
            var refreshToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1440),
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
                );
            var tokenHandle = new JwtSecurityTokenHandler();
            var refreshTokenString = tokenHandle.WriteToken(refreshToken);
            return refreshTokenString;
        }
    }
}
