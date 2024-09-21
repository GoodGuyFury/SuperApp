using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GetUserDataRepository;
using UserAuthRepository;
using AuthTokenRepository;
using AuthModel;


namespace MySuparApp.Controllers.Login
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly AuthToken _authToken;
        public LoginController(AuthToken authToken) // Inject AuthToken
        {
            _authToken = authToken;
        }

        [HttpPost("directlogin")]
        public async Task<IActionResult> DirectLogin([FromForm] string username, [FromForm] string password)
        {   
            var result = new AuthenticationResult();
            try
            {
                // Validate the username and password using GetUserDetailsFromExcel method asynchronously
                var userData = await Task.Run(() => GetUserData.GetUserDetailsFromExcel(userId: username, pass: password));

                // If userData is found and valid, generate a token
                if (userData != null && userData.UserId == username && !string.IsNullOrEmpty(userData.FirstName))
                {
                    // Generate the JWT token asynchronously if applicable
                    var jwt = await Task.Run(() => _authToken.GenerateToken(userData.Email, userData.FirstName, userData.Role, userData.UserId));

                    // Append the JWT as an HttpOnly cookie (this can remain synchronous)
                    UserAuthentication.AppendHttpOnlyCookie(HttpContext, "token-1", jwt);

                    result.verificationResult.status = "authorized";
                    result.verificationResult.message = "successful";
                    result.userInfo = new UserModel
                    {
                        UserId = userData.UserId,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        Role = userData.Role,
                        Email = userData.Email
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
