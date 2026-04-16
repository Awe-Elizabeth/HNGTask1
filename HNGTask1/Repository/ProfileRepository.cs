using HNGTask1.Data;
using HNGTask1.DTO;
using HNGTask1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Threading.Tasks;

namespace HNGTask1.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDBContext _context;
        public ProfileRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Profile> AddProfile(Profile profile)
        {
            try
            {
                await _context.Profiles.AddAsync(profile);
                await _context.SaveChangesAsync();
                return profile;
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        public async Task<int> DeleteProfile(Guid id)
        {
            return await _context.Profiles.Where(x => x.Id == id)
                                    .ExecuteDeleteAsync();

        }

        public async Task<Profile> GetByName(string Name)
        {

            return await _context.Profiles.FirstOrDefaultAsync<Profile>(p => p.Name == Name);

        }

        public async Task<Profile> GetOneProfile(Guid id)
        {
           return await _context.Profiles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ProfilesDTO>> GetProfiles(string? gender, string? country_id, string? age_group)
        {
            var query = _context.Profiles.AsQueryable();
            if(!string.IsNullOrWhiteSpace(gender))
            {
                query = query.Where(p => p.Gender.ToLower() == gender.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(country_id))
            {
                query = query.Where(p => p.Country_id.ToLower() == country_id.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(age_group))
            {
                query = query.Where(p => p.Age_group.ToLower() == age_group.ToLower());
            }
            return await query.Select(p => new ProfilesDTO
            {
                Id = p.Id,
                Name = p.Name,
                Gender = p.Gender,
                Age = p.Age,
                Age_group = p.Age_group,
                Country_id = p.Country_id,
            }).ToListAsync();
        }
    }
}
