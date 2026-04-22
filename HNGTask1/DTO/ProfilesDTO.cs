namespace HNGTask1.DTO
{
    public class ProfilesDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public double gender_probability { get; set; }
        public int sample_size { get; set; }
        public int? age { get; set; }
        public string age_group { get; set; }
        public string country_id { get; set; }
        public string country_name { get; set; }
        public double country_probability { get; set; }
        public DateTime created_at { get; set; }
    }
}
