using System.Text.Json;
using System.Security.Claims;
using MySuparApp.Models.Authentication;
using MySuparApp.Repository.Authentication;
using Microsoft.Extensions.Options;
using MySuparApp.Models.Shared;

namespace MySuparApp.Shared
{
    public class AuthHandler : IMiddleware
    {
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IGetUserData _getUserData;
        private readonly IAuthToken _authToken;
        private readonly ICurrentUserService _currentUserService;

        // Inject IOptions<JwtSettings> via the constructor
        public AuthHandler(IOptions<JwtSettings> jwtSettings, IGetUserData getUserData, IAuthToken authToken, ICurrentUserService currentuserservice)
        {
            _jwtSettings = jwtSettings;
            _getUserData = getUserData;
            _authToken = authToken;
            _currentUserService = currentuserservice;
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

            var token = context.Request.Cookies["auth-token"];

            // Check if token is missing
            if (string.IsNullOrEmpty(token))
            {
                await HandleUnauthorizedAsync(context, "User needs to authorize");
                return;
            }

            // Verify token and get claims
            var claimsPrincipal = _authToken.VerifyToken(token);

            // Check if token is valid and claims contain necessary information

            if (claimsPrincipal != null)
            {
                var currentUser = new CurrentUser
                {

                    Email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                    FirstName = claimsPrincipal.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
                    LastName = claimsPrincipal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
                    Role = Enum.TryParse<UserRole>(claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value, true, out var role) ? role : UserRole.User,
                    UserId = claimsPrincipal.FindFirst("UserId")?.Value ?? string.Empty
                };
                var jwtVerifiedClaim = claimsPrincipal.FindFirst("JWTVerified")?.Value; // Use the correct claim name


                bool jwtVerified = bool.TryParse(jwtVerifiedClaim, out var isVerified) && isVerified;

                if (jwtVerified)
                {
                    if (string.IsNullOrEmpty(currentUser.UserId) || string.IsNullOrEmpty(currentUser.Email) || string.IsNullOrEmpty(currentUser.FirstName))
                    {
                        await HandleUnauthorizedAsync(context, "User missing key details like UserId / email / First Name");
                        return;
                    }
                    context.Items["UserData"] = currentUser;

                    _currentUserService.SetCurrentUser(currentUser);
                    context.Items["UserData"] = currentUser;
                    // Allow access to initialization endpoint
                    if (path == "/appinitialize")
                    {
                        await HandleSuccessAsync(context, "Success", currentUser);
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
            var response =  VerificationResultDto.CreateError(message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleSuccessAsync(HttpContext context, string message, UserModel userData)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            // Fetch additional user details
            var userDetails = await _getUserData.GetUserDetails(userData.Email, authToken:true);
            userData = userDetails;

            var response = VerificationResultDto.CreateSuccess(userData);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response , options));
        }
    }
}
