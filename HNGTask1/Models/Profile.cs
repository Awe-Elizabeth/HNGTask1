using System.Data;

namespace HNGTask1.Models
{
    public class Profile
    {
       public Guid Id { get; set; } = Guid.NewGuid();
       public string Name { get; set; }
        public string Gender { get; set; }
        public double Gender_probability { get; set; }
        public int Sample_size { get; set; }
        public int? Age { get; set; }
        public string Age_group { get; set; }
        public string Country_id { get; set; }
        public double Country_probability { get; set; }
        public DateTime Created_at { get; set; }

    }
}
