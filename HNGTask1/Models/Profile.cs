namespace HNGTask1.Models
{
    public class Profile
    {
       public Guid Id { get; set; } = Guid.NewGuid();
       public string Name { get; set; }
        public string Gender { get; set; }
        public string Gender_Probability { get; set; }
        public int Sample_Size { get; set; }
        public string Age { get; set; }
        public string Age_group { get; set; }
        public string Country_id { get; set; }
        public int Country_probability { get; set; }
        public DateTime Created_at { get; set; }

    }
}
