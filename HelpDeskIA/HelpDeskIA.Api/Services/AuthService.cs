using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;

namespace HelpDeskIA.Api.Services {
    public class AuthService {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config) {
            _config = config;
        }

        public string HashPassword(string password) {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hash, string password) {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string GenerateJwtToken(int userId, string email, string role) {
            var key = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not configured");
            var issuer = _config["Jwt:Issuer"] ?? "HelpDeskIA";
            var audience = _config["Jwt:Audience"] ?? "HelpDeskIAUsers";
            var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"] ?? "120");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expireMinutes), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
