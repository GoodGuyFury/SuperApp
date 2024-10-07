using Microsoft.AspNetCore.Mvc;
using MySuparApp.Repository.Admin; // Adjust based on your structure
using MySuparApp.Models.Authentication; // Assuming UserModel is defined here
using System;
using System.Threading.Tasks;
using MySuparApp.Shared;
using Azure.Core;

namespace MySuparApp.Controllers.Admin
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        public class UpdateUserPasswordRequest
        {
            public UserModel? userModel { get; set; }
            public string? password { get; set; }
        }

        private readonly IUserManagement _usermanagement;
        private readonly ICurrentUserService _currentUserService;

        public AdminController(IUserManagement usermanagement, ICurrentUserService currentUserService)
        {
            _usermanagement = usermanagement;
            _currentUserService = currentUserService;
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
                return StatusCode(500, ResultStatusDto<UserModel>.CreateError(ex.Message));
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
                return StatusCode(500, ResultStatusDto<UserModel>.CreateError(ex.Message));
            }
        }
        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserAsync([FromBody] NewUserModel user)
        {
            try
            {
                var result = await _usermanagement.AddUserAsync(user);
                return Ok(ResultStatusDto<NewUserModel>.CreateSuccess("User created successfully", user));
            }
            catch (Exception ex)
            {

                return StatusCode(500, ResultStatusDto<object>.CreateError(ex.Message));
            }
        }
        [HttpPost("deleteuser")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] UserModel? user = null)
        {
            try {
                var currentUser = _currentUserService.GetCurrentUser();

                if (currentUser == null)
                {
                    return Unauthorized(ResultStatusDto<UserModel>.CreateError("User needs to be authorized")); // User must be authenticated
                }

                // Check if the current user is an admin or super admin
                bool isAdminOrSuperAdmin = currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.SuperAdmin;

                // If the current user is not an admin or super admin
                if (!isAdminOrSuperAdmin)
                {
                    // If the user parameter is provided, deny the request
                    if (user != null)
                    {
                        return Forbid(); // Only admins can delete other users
                    }

                    // Allow the user to delete themselves
                    user = currentUser;
                }

                if (user == null)
                {
                    return BadRequest(ResultStatusDto<object>.CreateError("User is required for deletion"));
                }

                var result = await _usermanagement.DeleteUserAsync(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResultStatusDto<object>.CreateError(ex.Message));
            }
        }

        [HttpPost("updatepassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordRequest updatePassModel)
        {
            try
            {
                if (updatePassModel.userModel == null || string.IsNullOrEmpty(updatePassModel.password))
                {
                    return BadRequest(ResultStatusDto<UserModel>.CreateError("Password succesfully updated"));
                }

                var result = await _usermanagement.SetPassword(updatePassModel.userModel, updatePassModel.password);
                return Ok(ResultStatusDto<UserModel>.CreateSuccess("Password succesfully updated"));
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ResultStatusDto<UserModel>.CreateError(ex.Message));
            }
        }
    }
}
