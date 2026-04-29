using HNGTask1.Models;

namespace HNGTask1.Repository.Interfaces
{
    public interface ITokenRepository
    {
        Task<RefreshToken> AddToken(RefreshToken token);
        Task<RefreshToken> GetToken(string token);
        Task<bool> UpdateToken(string token);
    }
}
