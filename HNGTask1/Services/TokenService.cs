using HNGTask1.DTO;
using HNGTask1.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HNGTask1.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public TokenResponse GenerateTokens(string userId, string role)
        {
            var accessToken = GenerateAccessToken(userId, role, out DateTime expiry);
            var refreshToken = GenerateRefreshToken();

            return new TokenResponse
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                token_expiry = expiry
            };
        }

        public string GenerateAccessToken(string userId, string role, out DateTime expiry)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:secret"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

                expiry = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("jwt:access_token_exp"));

                var token = new JwtSecurityToken(
                    issuer: _config["jwt:issuer"],
                    audience: _config["jwt:audience"],
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds
                );
                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                return accessToken;
            }
            catch (Exception ex)
            {
               Console.WriteLine("error createing token", ex.ToString());
                expiry = DateTime.UtcNow;
                return "";
            }
           
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }
    }
}
