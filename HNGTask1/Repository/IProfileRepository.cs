using HNGTask1.DTO;
using HNGTask1.Models;

namespace HNGTask1.Repository
{
    public interface IProfileRepository
    {
        Task<Profile> AddProfile(Profile profile);

        Task<PaginatedResult> GetProfiles( string? gender, string? country_id, string? age_group, int? min_age, int? max_age, double? min_gender_probability, double? min_country_probability, string? sortby, string? order, int page = 1, int limit = 10);
        //Task<PaginatedResult> GetProfileBySearch(string? gender, string? country_id, string? age_group, int? min_age, int? max_age)
        Task<Profile> GetOneProfile(Guid id);
        Task<Profile> GetByName(string Name);
        Task<int> DeleteProfile(Guid id);

    }
}
