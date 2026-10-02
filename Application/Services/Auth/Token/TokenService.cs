using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            var jwt = _configuration.GetSection("Jwt");

            var keyString = jwt["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            if (keyString.Length < 32)
            {
                throw new InvalidOperationException("JWT Key must be at least 32 characters long for security reasons.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var expireMinutesString = jwt["AccessTokenExpireMinutes"] ?? throw new InvalidOperationException("JWT AccessTokenExpireMinutes is not configured.");
            if (!double.TryParse(expireMinutesString, out double expireMinutes))
            {
                throw new InvalidOperationException("JWT AccessTokenExpireMinutes is not a valid number.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }

            if (user.OrganizationId != 0)
            {
                claims.Add(new Claim("OrganizationId", user.OrganizationId.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = jwt["Issuer"],
                Audience = jwt["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public int GetRefreshTokenExpiryDays()
        {
            var jwt = _configuration.GetSection("Jwt");
            var refreshTokenDaysString = jwt["RefreshTokenExpireDays"] ?? "7";

            if (!int.TryParse(refreshTokenDaysString, out int days))
            {
                days = 7;
            }

            return days;
        }

        public int GetAccessTokenExpiryMinutes()
        {
            var jwt = _configuration.GetSection("Jwt");
            var expireMinutesString = jwt["AccessTokenExpireMinutes"] ?? "60";

            if (!int.TryParse(expireMinutesString, out int minutes))
            {
                minutes = 60;
            }

            return minutes;
        }
    }
}