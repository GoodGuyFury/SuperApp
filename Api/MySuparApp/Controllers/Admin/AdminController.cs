using Microsoft.AspNetCore.Mvc;
using MySuparApp.Repository.Admin; // Adjust based on your structure
using MySuparApp.Models.Authentication; // Assuming UserModel is defined here
using System;
using System.Threading.Tasks;

namespace MySuparApp.Controllers.Admin
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _usermanagement;

        public AdminController(IAdminRepository usermanagement)
        {
            _usermanagement = usermanagement;
        }

        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUserAsync([FromForm] UserModel model)
        {
            try
            {
                await _usermanagement.CreateUserAsync(model);
                return Ok("User created successfully");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("fetchuserlist")]
        public async Task<IActionResult> FetchUserListAsync([FromBody] string searchText)
        {
            try
            {
                var users = await _usermanagement.GetUsersAsync(searchText);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("updateuser")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserModel user)
        {
            try
            {
                var result = await _usermanagement.UpdateUserAsync(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
