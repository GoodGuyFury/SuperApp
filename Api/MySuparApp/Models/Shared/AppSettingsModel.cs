namespace MySuparApp.Models.Shared
{
    public static class GoogleSettings
    {
        public static string GoogleClientId { get; set; } =string.Empty;
    }
    public class JwtSettings
    {
        public  string SecretKey { get; set; } = string.Empty;
        public  string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public  int TokenExpiryInMinutes { get; set; }
    }
}
