using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading.Tasks;
using userAuthModel;
using GoogleSigninTokenVerification;
using WebConfiguration;
using UserDataRepository;

namespace UserAuthRepository
{
    public class UserAuthentication
    {
        public async static Task<AuthenticationResult> AuthenticateUser(string jwt)
        {
            AuthenticationResult result = new AuthenticationResult();

            try
            {
                var Id = await GoogleTokenVerifier.VerifyGoogleTokenAndGetEmailAsync(jwt, WebConfig.GoogleClientId);

                if (Id.emailVerified)
                {
                    var userDetails =  GetUserDataRepository.GetUserDetailsFromExcel(Id.email);

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
    }
}
