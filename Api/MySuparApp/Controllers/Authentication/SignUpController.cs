using Microsoft.AspNetCore.Mvc;
using MySuparApp.Models.Authentication;
using MySuparApp.Repository.Admin;
using MySuparApp.Shared;

namespace MySuparApp.Controllers.Authentication
{
    [ApiController]
    [Route("signup")]
    public class SignUpController : ControllerBase
    {
        private readonly IUserManagement _usermanagement;
        private readonly ICurrentUserService _currentUserService;

        public SignUpController(IUserManagement usermanagement, ICurrentUserService currentUserService)
        {
            _usermanagement = usermanagement;
            _currentUserService = currentUserService;
        }

        [HttpGet(Name = "directsignup")]
        public IActionResult DirectSignUp([FromBody] UserModel user)
        {
             // Call the instance method
            return BadRequest(ResultStatusDto<object>.CreateError("Logout attempted, but 'auth-token' cookie was not found."));
        }
    }
}
