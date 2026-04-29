using HNGTask1.Data;
using HNGTask1.DTO;
using HNGTask1.Models;
using HNGTask1.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Threading.Tasks;

namespace HNGTask1.Repository.Implemetations
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
            return await _context.Profiles.Where(x => x.id == id)
                                    .ExecuteDeleteAsync();

        }

        public async Task<Profile> GetByName(string Name)
        {

            return await _context.Profiles.FirstOrDefaultAsync(p => p.name == Name);

        }

        public async Task<Profile> GetOneProfile(Guid id)
        {
           return await _context.Profiles.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<PaginatedResult> GetProfiles(string? gender, string? country_id, string? age_group, int? min_age, int? max_age, double? min_gender_probability, double? min_country_probability, string? sortby, string? order, int page = 1, int limit = 10)
        {
            var query = _context.Profiles.AsQueryable();
            if(!string.IsNullOrWhiteSpace(gender))
            {
                query = query.Where(p => p.gender.ToLower() == gender.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(country_id))
            {
                query = query.Where(p => p.country_id.ToLower() == country_id.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(age_group))
            {
                query = query.Where(p => p.age_group.ToLower() == age_group.ToLower());
            }
            if (min_age.HasValue)
            {
                query = query.Where(p => p.age >= min_age);
            }
            if (max_age.HasValue)
            {
                query = query.Where(p => p.age <= max_age);
            }
            if (min_gender_probability.HasValue)
            {
                query = query.Where(p => p.gender_probability >= min_gender_probability);
            }
            if (min_country_probability.HasValue)
            {
                query = query.Where(p => p.country_probability >= min_country_probability);
            }

            if (!string.IsNullOrWhiteSpace(sortby))
            {
                bool isDesc = order?.ToLower() == "desc";
                query = sortby.ToLower() switch
                {
                    "age" => isDesc ? query.OrderByDescending(p => p.age) : query.OrderBy(p => p.age),
                    "created_at" => isDesc ? query.OrderByDescending(p => p.created_at) : query.OrderBy(p => p.created_at),
                    "gender_probability" => isDesc ? query.OrderByDescending(p => p.gender_probability) : query.OrderBy(p => p.gender_probability),
                    _ => query.OrderBy(p => p.created_at)
                };
            }
            /*
                Pagination
            */

            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;
            limit = limit > 50 ? 50 : limit;

            var skip = (page - 1) * limit;

            var totalCount = await query.CountAsync();

            var data = await query.Skip(skip).Take(limit).Select(p => new ProfilesDTO
            {
                id = p.id,
                name = p.name,
                gender = p.gender,
                gender_probability = p.gender_probability,
                age = p.age,
                age_group = p.age_group,
                country_id = p.country_id,
                country_name = p.country_name,
                country_probability = p.country_probability,
                created_at = p.created_at,
            }).ToListAsync();

            return new PaginatedResult()
            {
                Page = page,
                Limit = limit,
                TotalCount = totalCount,
                Profiles = data
            };
        }
    }
}
