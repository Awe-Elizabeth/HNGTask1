using HNGTask1.Models;

namespace HNGTask1.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User> GetUserByGithubId(string id);
        Task<User> GetUserById(Guid id);

    }
}
