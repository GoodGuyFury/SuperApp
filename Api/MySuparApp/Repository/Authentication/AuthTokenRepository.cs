using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using AppSettingsModel;

namespace AuthTokenRepository
{
    public class AuthToken
    {
        private readonly JwtSettings _jwtSettings;

        public AuthToken(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        // Method to generate JWT token
        public string GenerateToken(string email, string name, string role, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("Username", username),
                    new Claim("JWTVerified", "true")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryInMinutes),
                Issuer = _jwtSettings.Issuer, // Use issuer from settings
                Audience = _jwtSettings.Audience, // Use audience from settings
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Method to verify and decode JWT token
        public ClaimsPrincipal? VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer, // Validate against issuer from settings
                    ValidAudience = _jwtSettings.Audience, // Validate against audience from settings
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null; // Token validation failed
            }
        }

        public bool RemoveAuthTokenCookie(HttpRequest request, HttpResponse response)
        {
            if (request.Cookies.ContainsKey("token-1"))
            {
                response.Cookies.Append("token-1", "", new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    Secure = true,
                    Path = "/",
                    SameSite = SameSiteMode.None
                });
                return true;
            }
            return false;
        }
    }
}
