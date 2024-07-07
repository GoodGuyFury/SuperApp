using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace MySuparApp
{
    public class AuthMiddlware:IMiddleware

    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Fetch headers and cookies from the request
            var myHeader = context.Request.Headers["MyCustomHeader"];
            var myCookie = context.Request.Cookies["myjwt"];

            // Perform any verification or additional processing here

            // Call the next middleware in the pipeline
            await next(context);
        }
    }
}

