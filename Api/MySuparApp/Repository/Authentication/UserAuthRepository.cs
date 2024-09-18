using MySuparApp.Models.Authentication;
using WebConfiguration;
using MySuparApp.Repository.GetUserData;

namespace MySuparApp.Repository.UserAuth
{
    public class UserAuthentication
    {
        public async static Task<AuthenticationResult> AuthenticateUser(string jwt)
        {
            AuthenticationResult result = new AuthenticationResult();

            try
            {
                var Id = await GoogleTokenVerifier.GoogleTokenVerifier.VerifyGoogleTokenAndGetEmailAsync(jwt, WebConfig.GoogleClientId);

                if (Id.emailVerified)
                {
                    var userDetails = GetUserDataRepository.GetUserDetailsFromExcel(UserEmail: Id.email);

                    if (userDetails != null)
                    {
                        result.userInfo = userDetails;
                        result.verificationResult.status = "authorized";
                        result.verificationResult.message = "Successful";
                    }
                    else
                    {
                        result.verificationResult.status = "unauthorized";
                        result.verificationResult.message = "Authorization failed";
                    }
                }
                else
                {
                    result.verificationResult.status = "error";
                    result.verificationResult.message = Id.msg;
                }
            }
            catch (Exception ex)
            {
                result.verificationResult.status = "error";
                result.verificationResult.message = ex.Message;
            }

            return result;
        }


        public static void AppendHttpOnlyCookie(HttpContext httpContext, string CookieName, string jwt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddHours(1), // Adjust as needed
                Path = "/",
                SameSite = SameSiteMode.None
            };

            httpContext.Response.Cookies.Append(CookieName, jwt, cookieOptions);
        }
        public static void RemoveHttpOnlyCookie(HttpContext httpContext, string cookieName)
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
        public static void ExpireHttpOnlyCookie(HttpContext httpContext, string cookieName)
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
