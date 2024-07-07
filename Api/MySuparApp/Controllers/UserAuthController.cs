using Microsoft.AspNetCore.Mvc;
using UserAuthRepository;


namespace UserAuthController;

[ApiController]
[Route("[controller]")]
public class SignInWithGoogle : ControllerBase
{

    private readonly ILogger<SignInWithGoogle> _logger;

    public SignInWithGoogle(ILogger<SignInWithGoogle> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "SignInWithGoogle")]
    public async Task<IActionResult> verifyUserdata()
    {
        if (Request.Headers.TryGetValue("googjwt", out var jwtHeaderValue))
        {
            string jwt = jwtHeaderValue.ToString();

            var returnVal = await UserAuthentication.ReadExcelFile(jwt);

            switch (returnVal.VerificationResult.Status.ToLower())
            {
                case "authorized":

                    UserAuthentication.AppendHttpOnlyCookie(HttpContext,"token-1", jwt);

                    return Ok(returnVal);
                case "unauthorized":
                    return Unauthorized(returnVal);
                case "error":
                    return StatusCode(500); // Internal Server Error
                default:
                    return BadRequest("Unknown status"); // or handle accordingly
            }
        }
        else
        {
            return Unauthorized("Missing or invalid 'googjwt' header.");
        }

    }

}
