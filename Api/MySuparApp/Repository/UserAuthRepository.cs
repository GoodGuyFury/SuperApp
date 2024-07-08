using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading.Tasks;
using UserAuthModel;
using GoogleSigninTokenVerification;
using WebConfiguration;
using UserDataRepository;

namespace UserAuthRepository
{
    public class AuthenticationResult
    {
        public VerificationResultDto VerificationResult { get; set; }
        public UserInfo? UserInfo { get; set; }

        public AuthenticationResult()
        {
            VerificationResult = new VerificationResultDto();
        }
    }

    public class UserAuthentication
    {
        public async static Task<AuthenticationResult> AuthenticateUser(string jwt)
        {
            AuthenticationResult result = new AuthenticationResult();

            try
            {
                var Id = await GoogleTokenVerifier.VerifyGoogleTokenAndGetEmailAsync(jwt, WebConfig.GoogleClientId);

                if (Id.EmailVerified)
                {
                    var userDetails =  GetUserDataRepository.GetUserDetailsFromExcel(Id.Email);

                    if (userDetails != null)
                    {
                        result.UserInfo = userDetails;
                        result.VerificationResult.Status = "authorized";
                        result.VerificationResult.Message = "Successful";
                    }
                    else
                    {
                        result.VerificationResult.Status = "unauthorized";
                        result.VerificationResult.Message = "Authorization failed";
                    }
                }
                else
                {
                    result.VerificationResult.Status = "error";
                    result.VerificationResult.Message = Id.msg;
                }
            }
            catch (Exception ex)
            {
                result.VerificationResult.Status = "error";
                result.VerificationResult.Message = ex.Message;
            }

            return result;
        }


        public static void AppendHttpOnlyCookie(HttpContext httpContext, string CookieName, string jwt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // if you want the cookie to be sent only over HTTPS
                Expires = DateTimeOffset.Now.AddDays(1),
                // Path = "/", // the path for which the cookie is valid
                // Domain = "yourdomain.com" // the domain for which the cookie is valid
            };

            httpContext.Response.Cookies.Append(CookieName, jwt, cookieOptions);
        }
    }
}
