using HNGTask1.DTO;

namespace HNGTask1.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, string roles, out DateTime expiry);
        string GenerateRefreshToken();
        TokenResponse GenerateTokens(string userId, string role);
    }
}