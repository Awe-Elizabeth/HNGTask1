namespace HNGTask1.DTO
{
    public class Nationalize
    {
        public int count {  get; set; }    
        public string name { get; set; }
        public List<NationalizeList>? Country { get; set; }
    }

    public class NationalizeList
    {
        public string Country_id { get; set; }
        public double Probability { get; set; }
    }
}
