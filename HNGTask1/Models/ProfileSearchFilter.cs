namespace HNGTask1.Models
{
    public class ProfileSearchFilter
    {
        public string? Gender { get; set; }
        public string? CountryId { get; set; }
        public string? AgeGroup { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }
}
