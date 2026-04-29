using HNGTask1.Data;
using HNGTask1.Models;
using HNGTask1.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HNGTask1.Repository.Implemetations
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AppDBContext _context;
        public TokenRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<RefreshToken> AddToken(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken> GetToken(string token)
        {
            return await _context.RefreshTokens
                                    .FirstOrDefaultAsync(x =>
                                    x.token == token && 
                                    x.is_valid == true &&
                                    x.expires_at < DateTime.UtcNow
                                    ); 
        }

        public async Task<bool> UpdateToken(string token)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.token == token);
            refreshToken.is_valid = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
