using MySuparApp.Models.Shared;
using MySuparApp.Models.Authentication;
using MySuparApp.Repository.Authentication;

namespace MySuparApp.Repository.Authentication
{   public interface ICookieHandler{
        void AppendHttpOnlyCookie(HttpContext httpContext, string CookieName, string jwt);
        void RemoveHttpOnlyCookie(HttpContext httpContext, string cookieName);
        void ExpireHttpOnlyCookie(HttpContext httpContext, string cookieName);
    }
    public class CookieHandler : ICookieHandler
    {
        public readonly IGetUserData _getUserData;

        public CookieHandler(IGetUserData getUserData) {
            _getUserData = getUserData;
        }


        public void AppendHttpOnlyCookie(HttpContext httpContext, string CookieName, string jwt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddHours(2), // Adjust as needed
                Path = "/",
                SameSite = SameSiteMode.None
            };

            httpContext.Response.Cookies.Append(CookieName, jwt, cookieOptions);
        }
        public void RemoveHttpOnlyCookie(HttpContext httpContext, string cookieName)
        {
            // Use Delete method to remove the cookie
            httpContext.Response.Cookies.Delete(cookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",  // Ensure this matches the path when the cookie was created
                SameSite = SameSiteMode.None
            });
        }
        public void ExpireHttpOnlyCookie(HttpContext httpContext, string cookieName)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set to the past
                Path = "/",
                SameSite = SameSiteMode.None
            };

            httpContext.Response.Cookies.Append(cookieName, string.Empty, cookieOptions);
        }


    }
}
