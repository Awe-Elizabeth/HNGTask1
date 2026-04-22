
using HNGTask1.DTO;
using HNGTask1.Models;
using System.Text.Json;

namespace HNGTask1.Data.Seed
{
    public class ProfileSeeder : ISeeder
    {
        private readonly ILogger _logger;
        public ProfileSeeder(ILogger<ProfileSeeder> logger)
        {
            _logger = logger;
        }
        public async Task SeedAsync(AppDBContext c)
        {
            try
            {
                if (c.Profiles.Any())
                {
                    return;
                }

                var json = await File.ReadAllTextAsync("Data/Seed/profiles.json");

                var root =  JsonSerializer.Deserialize<ProfileSeedRoot>(json);
                if (root?.profiles == null || !root.profiles.Any())
                    return;
                c.Profiles.AddRange(root.profiles);
                await c.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured: {ex.ToString()}"); 
            } 
        }
    }
}
