using HNGTask1.Models;
using Microsoft.EntityFrameworkCore;

namespace HNGTask1.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public DbSet<Profile> Profiles {get; set;}
    }
}
