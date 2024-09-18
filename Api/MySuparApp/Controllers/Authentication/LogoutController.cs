using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MySuparApp.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]")]
    public class LogoutController : ControllerBase
    {
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(ILogger<LogoutController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "logout")]
        public IActionResult Logout()
        {
            // Check if the "token-1" cookie exists
            if (Request.Cookies.ContainsKey("token-1"))
            {
                // Remove the cookie by setting it with an expired timestamp

                Response.Cookies.Append("token-1", "", new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    Secure = true,
                    Path = "/",
                    SameSite = SameSiteMode.None
                });

                _logger.LogInformation("User logged out and 'token-1' cookie removed.");
                return Ok("Logged out successfully.");
            }

            _logger.LogWarning("Logout attempted, but 'token-1' cookie was not found.");
            return BadRequest("No 'token-1' cookie found.");
        }
    }
}
