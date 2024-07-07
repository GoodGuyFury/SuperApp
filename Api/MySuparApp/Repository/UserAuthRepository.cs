using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using UserAuthModel;
using GoogleSigninTokenVerification;
using WebConfiguration;

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
        public async static Task<AuthenticationResult> ReadExcelFile(string jwt)
        {
            var GoogleJwtVerifier = new GoogleTokenVerifier();
            string googjwt = jwt;
            var stat = new VerificationResultDto();
            AuthenticationResult result = new AuthenticationResult();
            try
            {

                var Id = await GoogleJwtVerifier.VerifyGoogleTokenAndGetEmailAsync(googjwt, WebConfig.GoogleClientId);
                if (Id.EmailVerified)
                {
                    string filePath = "Main DB\\UserList.xlsx";

                    FileInfo fileInfo = new FileInfo(filePath);

                    using (ExcelPackage package = new ExcelPackage(fileInfo))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                  
                        bool idFound = false;
                        
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var cellValue = worksheet.Cells[row, 2].Text;

                            
                            if (cellValue == Id.Email)
                            {
                                var userRoles = new UserInfo
                                {
                                    FullName = worksheet.Cells[row, 3].Text,
                                    Role = worksheet.Cells[row, 5].Text,
                                    userId = worksheet.Cells[row, 1].Text,
                                    Message = worksheet.Cells[row, 4].Text,
                                    Email = worksheet.Cells[row, 2].Text
                                };

                                result.UserInfo = userRoles;
                                idFound = true;
                                break;
                            }
                        }
                        if (idFound)
                        {

                            result.VerificationResult.Status = "authorized";
                            result.VerificationResult.Message = "Successful";
                            return (result) ;
                        }
                        else
                        {
                            result.VerificationResult.Status = "unauthorized";
                            result.VerificationResult.Message = "authorization failed";
                            return (result);
                        }
                    }
                }
                else
                {
                    result.VerificationResult.Status = "error";
                    result.VerificationResult.Message = Id.msg;
                    return (result);
                }
            }
            catch (Exception ex)
            {
                result.VerificationResult.Message = ex.Message;
                result.VerificationResult.Status = "error";
                return (result);
            }
        }
        public static void AppendHttpOnlyCookie(HttpContext httpContext, string CookieName, string jwt)
        {
            // Create the cookie options
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Set the HttpOnly property
                                 // Optionally, you can set other properties like:
                                  Secure = true, // if you want the cookie to be sent only over HTTPS
                Expires = DateTimeOffset.Now.AddDays(1), // if you want the cookie to have an expiration date
                                                         // Path = "/", // the path for which the cookie is valid
                                                         // Domain = "yourdomain.com" // the domain for which the cookie is valid
            };

            // Append the cookie to the response
            httpContext.Response.Cookies.Append(CookieName, jwt, cookieOptions);
        }
    }
}
