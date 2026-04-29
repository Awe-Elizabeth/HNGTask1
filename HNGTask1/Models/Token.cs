using Microsoft.Identity.Client;

namespace HNGTask1.Models
{
    public class RefreshToken
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public string token {  get; set; }
        public DateTime created_at { get; set; }
        public DateTime expires_at { get; set; }
        public bool is_valid { get; set; }
        public Guid user_id { get; set; }
    }
}
