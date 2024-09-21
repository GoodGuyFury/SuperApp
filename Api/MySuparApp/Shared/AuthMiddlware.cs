using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AuthModel;
using GetUserDataRepository;
using AuthTokenRepository;
using Microsoft.Extensions.Options;
using AppSettingsModel;

namespace AuthMiddlware
{
    public class AuthHandler : IMiddleware
    {
        private readonly IOptions<JwtSettings> _jwtSettings;

        // Inject IOptions<JwtSettings> via the constructor
        public AuthHandler(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.ToString().ToLower();

            // Allow unauthenticated access to specific paths
            if (path == "/login" || path == "/login/signinwithgoogle" || path == "/logout" || path == "/login/directlogin")
            {
                await next(context);
                return;
            }

            var token = context.Request.Cookies["token-1"];

            // Check if token is missing
            if (string.IsNullOrEmpty(token))
            {
                await HandleUnauthorizedAsync(context, "User needs to authorize");
                return;
            }

            // Verify token and get claims
            var authToken = new AuthToken(_jwtSettings);
            var claimsPrincipal = authToken.VerifyToken(token);

            // Check if token is valid and claims contain necessary information
            if (claimsPrincipal != null)
            {
                var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
                var username = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                var role = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
                var jwtVerifiedClaim = claimsPrincipal.FindFirst("JWTVerified")?.Value; // Use the correct claim name


                bool emailVerified = bool.TryParse(jwtVerifiedClaim, out var isVerified) && isVerified;

                if (emailVerified)
                {
                    var userData = new AuthenticationResult
                    {
                        userInfo = new UserModel
                        {
                            Email = email ?? string.Empty,
                            //UserId = username ?? string.Empty,
                            Role = role ?? string.Empty
                        },
                        verificationResult = new VerificationResultDto
                        {
                            status = "authorized", // Example value, you can adjust as needed
                            message = "successful" // Example value
                        }
                    };

                    context.Items["UserData"] = userData;

                    // Allow access to initialization endpoint
                    if (path == "/appinitialize")
                    {
                        await HandleSuccessAsync(context, "Success", userData);
                        return;
                    }

                    // Allow access to other authenticated endpoints
                    await next(context);
                    return;
                }
            }

            await HandleUnauthorizedAsync(context, "User needs to authorize");
        }

        private async Task HandleUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var response = new VerificationResultDto();
            response.status = "unauthorized";
            response.message = message;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleSuccessAsync(HttpContext context, string message, AuthenticationResult userData)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            // Fetch additional user details
            var userDetails = GetUserData.GetUserDetailsFromExcel(userData.userInfo.Email);
            userData.userInfo = userDetails;

            var response = new
            {
                userData.verificationResult,
                userData.userInfo
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
