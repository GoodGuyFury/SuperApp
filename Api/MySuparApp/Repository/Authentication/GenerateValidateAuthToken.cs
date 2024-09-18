
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    namespace MySuparApp.Repository.GenerateValidateToken
    {
        public class AuthTokenRepository
        {
            private const string SecretKey = "a3f7e2c8d3c45b9877c2e58d937b55b6f6d9b8c7d0e8c3f2b7b4d4d1234e56789"; // Replace with your actual key
            private const string Issuer = "SuperAppIssuer";
            private const string Audience = "SuperAppUser"; // Audience named SuperAppUser
            private const int TokenExpiryInMinutes = 10;

            // Method to generate JWT token
            public static string GenerateToken(string email, string name, string role, string username)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);

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
                    Expires = DateTime.UtcNow.AddMinutes(TokenExpiryInMinutes),
                    Issuer = Issuer, // Include Issuer
                    Audience = Audience, // Include Audience
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

            // Method to verify and decode JWT token
            public static ClaimsPrincipal? VerifyToken(string token)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);

                try
                {
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = Issuer, // Validate against the specified issuer
                        ValidAudience = Audience, // Validate against the specified audience
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
        }
    }

