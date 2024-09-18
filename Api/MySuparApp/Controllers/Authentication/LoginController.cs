using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySuparApp.Repository.Authentication.MySuparApp.Repository.Authentication;

namespace MySuparApp.Controllers.Authentication
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
            if (username == "admin" && password == "password") // Replace with actual validation logic
            {
                //var token = _authTokenRepository.GenerateToken(username, "Admin User", "ADMIN", username);
                //return Ok(new { Token = token });
            }
            return Unauthorized("Invalid username or password.");
        }
    }
}
