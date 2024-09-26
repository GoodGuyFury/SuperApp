namespace MySuparApp.Models.Shared
{
    public static class GoogleSettings
    {
        public static string GoogleClientId { get; set; }
    }
    public class JwtSettings
    {
        public  string SecretKey { get; set; }
        public  string Issuer { get; set; }
        public  string Audience { get; set; }
        public  int TokenExpiryInMinutes { get; set; }
    }
}
