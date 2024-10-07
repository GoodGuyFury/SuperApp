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
       
            if (authRemoved)
            {
                _logger.LogInformation("User logged out and 'auth-token' cookie removed.");
                return Unauthorized(ResultStatusDto<object>.CreateSuccess("User logged out and 'auth-token' cookie removed."));
            }
            _logger.LogWarning("Logout attempted, but 'auth-token' cookie was not found.");
            return BadRequest(ResultStatusDto<object>.CreateError("Logout attempted, but 'auth-token' cookie was not found."));
        }
    }
}
