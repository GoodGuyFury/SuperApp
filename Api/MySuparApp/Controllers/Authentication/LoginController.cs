using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySuparApp.Repository.GetUserData;
using MySuparApp.Repository.UserAuth;
using MySuparApp.Repository.GenerateValidateToken;
using MySuparApp.Models.Authentication;


namespace MySuparApp.Controllers.Login
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        //private readonly AuthTokenRepository _authTokenRepository;

        //public LoginController(AuthTokenRepository authTokenRepository)
        //{
        //    _authTokenRepository = authTokenRepository;
        //}

        [HttpPost("directlogin")]
        public async Task<IActionResult> DirectLogin([FromForm] string username, [FromForm] string password)
        {
            var result = new AuthenticationResult();
            try
            {
                // Validate the username and password using GetUserDetailsFromExcel method asynchronously
                var userData = await Task.Run(() => GetUserDataRepository.GetUserDetailsFromExcel(userId: username, pass: password));

                // If userData is found and valid, generate a token
                if (userData != null && userData.userId == username && !string.IsNullOrEmpty(userData.fullName))
                {
                    // Generate the JWT token asynchronously if applicable
                    var jwt = await Task.Run(() => AuthTokenRepository.GenerateToken(userData.email, userData.fullName, userData.role, userData.userId));

                    // Append the JWT as an HttpOnly cookie (this can remain synchronous)
                    UserAuthentication.AppendHttpOnlyCookie(HttpContext, "token-1", jwt);

                    result.verificationResult.status = "authorized";
                    result.verificationResult.message = "successful";
                    result.userInfo = new UserInfo
                    {
                        userId = userData.userId,
                        fullName = userData.fullName,
                        message = userData.message,
                        role = userData.role,
                        email = userData.email
                    };

                    // Return the AuthenticationResult model
                    return Ok(result);
                }

                // If credentials are invalid, set error status and message
                result.verificationResult.status = "error";
                result.verificationResult.message = "Invalid username or password.";
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                result.verificationResult.status = "error";
                result.verificationResult.message = "An unexpected error occurred: " + ex.Message;
                return StatusCode(500, result);
            }
        }


    }
}
