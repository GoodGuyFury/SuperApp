using Microsoft.AspNetCore.Mvc;
using MySuparApp.Repository.RoutingAccess;
using AuthModel;

namespace MySuparApp.Controllers.RoutingAccess
{
    public class RoutingController : Controller
    {
        private readonly ILogger<RoutingController> _logger;

        public RoutingController(ILogger<RoutingController> logger)
        {
            _logger = logger;
        }
        [HttpGet("routeaccess", Name = "RouteAccess")]
        public IActionResult RouteAccess()
        {
            try
            {
                if (HttpContext.Items.TryGetValue("UserData", out var userData))
                {
                    var user = userData as UserModel; // Ensure UserInfo is used here
                    if (user != null)
                    {

                        bool access = RoutingAceessRepository.GetAcess(user);
                        return Ok(access);
                    }
                    else
                    {
                        return Unauthorized();
                    }

                }
                return NotFound(); // Return NotFound if UserData is not found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in RouteAccess");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
