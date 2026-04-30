
using HNGTask1.Essential_Enums;
using HNGTask1.Models;
using System;
using System.Data;

namespace HNGTask1.Data.Seed
{
    public class SeedUsers : ISeeder
    {
        public async Task SeedAsync(AppDBContext context)
        {

            if (!context.Users.Any())
            {
                context.Users.AddRange(new User
                {
                    id = Guid.NewGuid(),
                    github_id = "12345678",
                    name = "TestAdmin",
                    email = "TestAdmin@example.com",
                    avatar_url = "testadmin.com",
                    role = UserEnum.admin.ToString(),
                    is_active = true,
                    last_login_at = DateTime.UtcNow,
                    created_at = DateTime.UtcNow
                },
                new User
                {
                    
                    id = Guid.NewGuid(),
                    github_id = "12345679",
                    name = "TestAnalyst",
                    email = "TestAnalyst@example.com",
                    avatar_url = "testanalysturl.com",
                    role = UserEnum.analyst.ToString(),
                    is_active = true,
                    last_login_at = DateTime.UtcNow,
                    created_at = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
