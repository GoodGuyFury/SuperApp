using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;
using WebConfiguration;
using MySuparApp.Repository.Authentication;
using MySuparApp.Models.Authentication;

namespace AuthMiddlware
{
    public class AuthHandler:IMiddleware

    {
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

            // Verify token and get email
            var data = await GoogleTokenVerifier.VerifyGoogleTokenAndGetEmailAsync(token, WebConfig.GoogleClientId);


            // Check if email is verified
            if (data.emailVerified)
            {
                var userData = new AuthenticationResult
                {
                    userInfo = new UserInfo
                    {
                        email = data.email ?? string.Empty    // Replace with actual data fetching logic
                    },
                    verificationResult = new VerificationResultDto
                    {
                        status = "authorized",    // Example value, you can replace this as needed
                        message = "Successfull" // Example value
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

        private async Task HandleSuccessAsync(HttpContext context, string message, AuthenticationResult userData)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            var userDetails = GetUserDataRepository.GetUserDetailsFromExcel(userData.userInfo.email);
            userData.userInfo = userDetails;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                userData.verificationResult,
                userData.userInfo
            }));
        }


    }
}

