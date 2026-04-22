using HNGTask1.Models;

namespace HNGTask1.DTO
{
    public class PaginatedResult
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
        public List<ProfilesDTO> Profiles { get; set; }
    }
}
