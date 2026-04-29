using Microsoft.AspNetCore.Identity;

namespace HNGTask1.Models
{
    public class User
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string github_id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? avatar_url { get; set; }
        public string role {  get; set; }
        public bool is_active { get; set; }
        public DateTime last_login_at { get; set; }
        public DateTime created_at { get; set; }
    }
}
