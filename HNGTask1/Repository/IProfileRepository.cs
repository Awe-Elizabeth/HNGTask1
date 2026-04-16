using HNGTask1.DTO;
using HNGTask1.Models;

namespace HNGTask1.Repository
{
    public interface IProfileRepository
    {
        Task<Profile> AddProfile(Profile profile);

        Task<List<ProfilesDTO>> GetProfiles(string? gender, string? country_id, string? age_group);
        Task<Profile> GetOneProfile(Guid id);
        Task<Profile> GetByName(string Name);
        Task<int> DeleteProfile(Guid id);

    }
}
