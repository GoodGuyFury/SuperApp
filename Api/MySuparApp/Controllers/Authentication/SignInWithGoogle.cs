using Microsoft.AspNetCore.Mvc;
using UserAuthRepository;



namespace MySuparApp.Controllers.SignInWithGoogle
{
    [ApiController]
    [Route("login")]
    public class SignInWithGoogle : ControllerBase
    {
        private readonly ILogger<SignInWithGoogle> _logger;

        public SignInWithGoogle(ILogger<SignInWithGoogle> logger)
        {
            _logger = logger;
        }

        [HttpGet("signinwithgoogle")]
        public async Task<IActionResult> VerifyUserData()
        {
            if (Request.Headers.TryGetValue("googjwt", out var jwtHeaderValue))
            {
                string jwt = jwtHeaderValue.ToString();

                // Assuming UserAuthentication.AuthenticateUser(jwt) exists and handles JWT validation
                var returnVal = await UserAuthentication.AuthenticateUser(jwt);

                switch (returnVal.verificationResult.status.ToLower())
                {
                    case "authorized":
                        // Example: Set a cookie or handle the authorized user scenario
                        UserAuthentication.AppendHttpOnlyCookie(HttpContext, "token-1", jwt);

                        return Ok(returnVal);
                    case "unauthorized":
                        return Unauthorized(returnVal);
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
