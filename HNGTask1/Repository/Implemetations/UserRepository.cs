using HNGTask1.Data;
using HNGTask1.Models;
using HNGTask1.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HNGTask1.Repository.Implemetations
{
    public class UserRepository(AppDBContext context) : IUserRepository
    {
        private readonly AppDBContext _context = context;

        public async Task<User> AddUser(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        public async Task<User> GetUserByGithubId(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.github_id == id);
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.id == id);

        }
    }
}
