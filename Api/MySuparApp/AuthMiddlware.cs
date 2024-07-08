using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;
using UserAuthController;
namespace AuthMiddlware
{
    public class AuthHandler:IMiddleware

    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.ToString().ToLower();

            if (path == "/login" || path== "/signinwithgoogle")
            {
                await next(context);
                return;
            }

            var token = context.Request.Cookies["token-1"];

            if (string.IsNullOrEmpty(token) )
            {
                if (path == "/appinitialize")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var response = new { message = "User needs to authorize" };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var response = new { message = "Unauthorized" };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
                return;
            }

            if (path == "/appinitialize")
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                var response = new { message = "Success" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            await next(context);
        }
    }
}

