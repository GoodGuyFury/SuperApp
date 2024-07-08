using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;
using UserAuthController;
using GoogleSigninTokenVerification;
using WebConfiguration;
using UserAuthModel;
using UserDataRepository;

namespace AuthMiddlware
{
    public class AuthHandler:IMiddleware

    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.ToString().ToLower();

            // Allow unauthenticated access to specific paths
            if (path == "/login" || path == "/signinwithgoogle")
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

            // Verify token and get email
            var data = await GoogleTokenVerifier.VerifyGoogleTokenAndGetEmailAsync(token, WebConfig.GoogleClientId);

            // Check if email is verified
            if (data.EmailVerified)
            {
                var userData = new UserInfo
                {
                    FullName = data.Name, // Replace with actual data fetching logic
                    Email = data.Email  // Replace with actual data fetching logic
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
            else
            {
                await HandleUnauthorizedAsync(context, "User needs to authorize");
                return;
            }
        }

        private async Task HandleUnauthorizedAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var response = new { message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleSuccessAsync(HttpContext context, string message, UserInfo userData)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            userData = GetUserDataRepository.GetUserDetailsFromExcel(userData.Email);
            var response = new { message, userData };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

    }
}

