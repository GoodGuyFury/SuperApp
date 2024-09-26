using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySuparApp.Repository.Authentication;
using MySuparApp.Models.Authentication;

namespace MySuparApp.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]")]
    public class LogoutController : ControllerBase
    {
        private readonly ILogger<LogoutController> _logger;
        private readonly IAuthToken _authToken; // Non-nullable field

        public LogoutController(ILogger<LogoutController> logger, IAuthToken authToken) // Single constructor
        {
            _logger = logger;
            _authToken = authToken; // Ensure it's not null
        }

        [HttpGet(Name = "logout")]
        public IActionResult Logout()
        {
            var authRemoved = _authToken.RemoveAuthTokenCookie(Request, Response); // Call the instance method
            var result = new VerificationResultDto();
            if (authRemoved)
            {
                result.status = "unathorized";
                result.message = "Logged Out Succesfully";
                _logger.LogInformation("User logged out and 'token-1' cookie removed.");
                return Unauthorized(result);
            }
            result.status = "bad request";
            result.message = "Logout attempted, but 'token-1' cookie was not found.";

            _logger.LogWarning("Logout attempted, but 'token-1' cookie was not found.");
            return BadRequest(result);
        }
    }
}
