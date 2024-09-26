using Microsoft.AspNetCore.Mvc;
using MySuparApp.Models.Authentication;
using MySuparApp.Models.Shared;
using MySuparApp.Repository.Authentication;

namespace MySuparApp.Controllers.Authentication
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthToken _authToken;
        private readonly IGetUserData _getUserData;
        private readonly IGoogleTokenVerifier _googleTokenVerifier;
        private readonly ICookieHandler _cookieHandler;

        public LoginController(IAuthToken authToken, IGetUserData userData, IGoogleTokenVerifier googleTokenVerifier,ICookieHandler cookieHandler) // Inject AuthToken
        {
            _authToken = authToken;
            _getUserData = userData;
            _googleTokenVerifier = googleTokenVerifier;
            _cookieHandler = cookieHandler;
        }

        [HttpPost("directlogin")]
        public async Task<IActionResult> DirectLogin([FromForm] string email, [FromForm] string password)
        {   
            var result = new AuthenticationResult();
            try
            {
                // Validate the username and password using GetUserDetailsFromExcel method asynchronously
                var userData = await Task.Run(() => _getUserData.GetUserDetails(email: email, pass: password));

                // If userData is found and valid, generate a token
                if (userData != null && userData.Email == email && !string.IsNullOrEmpty(userData.FirstName))
                {
                    // Generate the JWT token asynchronously if applicable
                    var jwt = await Task.Run(() => _authToken.GenerateToken(userData.UserId, userData.FirstName, userData.LastName,userData.Email, userData.Role));

                    // Append the JWT as an HttpOnly cookie (this can remain synchronous)
                    _cookieHandler.AppendHttpOnlyCookie(HttpContext, "token-1", jwt);

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

        [HttpGet("signinwithgoogle")]
        public async Task<IActionResult> VerifyUserData()
        {
            if (Request.Headers.TryGetValue("googjwt", out var jwtHeaderValue))
            {
                string jwt = jwtHeaderValue.ToString();
                AuthenticationResult result = new AuthenticationResult();

                try
                {
                    var Id = await _googleTokenVerifier.VerifyGoogleTokenAndGetEmailAsync(jwt, GoogleSettings.GoogleClientId);

                    if (Id.emailVerified)
                    {
                        var userDetails =await _getUserData.GetUserDetails(Id.email, authToken: true);

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

                switch (result.verificationResult.status.ToLower())
                {
                    case "authorized":
                        // Example: Set a cookie or handle the authorized user scenario
                        _cookieHandler.AppendHttpOnlyCookie(HttpContext, "token-1", jwt);

                        return Ok(result);
                    case "unauthorized":
                        return Unauthorized(result);
                    case "error":
                        return StatusCode(500, "An error occurred during authentication.");
                    default:
                        return BadRequest("Unknown verification status.");
                }
            }
            else
            {
                return Unauthorized("Missing or invalid 'googjwt' header.");
            }
        }


    }
}
