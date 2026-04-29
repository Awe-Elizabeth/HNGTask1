namespace HNGTask1.DTO
{
    public class TokenResponse
    {
        public string access_token {  get; set; }
        public string refresh_token { get; set; }
        public DateTime token_expiry { get; set; }
    }
}
