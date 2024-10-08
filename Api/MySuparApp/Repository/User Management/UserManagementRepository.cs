using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySuparApp.Models.Authentication; // Assuming UserModel is defined here
using MySuparApp.Shared;
using MySuperApp.Repository.Authentication; // Assuming ApplicationDbContext is defined here

namespace MySuparApp.Repository.Admin
{
   
    public interface IUserManagement
    {
        Task<List<UserModel>> GetUsersAsync(string searchText);
        Task<ResultStatusDto<UserModel>> UpdateUserAsync(UserModel userUpdateRequest, UserModel requestingUser, bool isSelfUpdate);
        Task<ResultStatusDto<NewUserModel>> AddUserAsync(NewUserModel newUser, UserModel? requestingUser = null);
        Task<ResultStatusDto<UserModel>> DeleteUserAsync(UserModel userToRemove, UserModel requestingUser, bool isSelfDelete);
        Task<ResultStatusDto<UserModel>> SetPassword(UserModel user, string newPassword, string? oldPassword = null, string? otp = null);
    }
    public class UserManagement : IUserManagement
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPasswordManager _passwordManager;
        private readonly IMapper _mapper;

        public UserManagement(ApplicationDbContext context, ICurrentUserService currentUserService, IPasswordManager passwordManager, IMapper mapper)
        {
            _context = context;
            _currentUserService = currentUserService;
            _passwordManager = passwordManager;
            _mapper = mapper;
        }

        public async Task<List<UserModel>> GetUsersAsync(string searchText)
        {
            try
            {
                // Start by querying the EntityUserModel which contains UserInt
                var query = _context.Users.AsQueryable();

                // If searchText is "↺", return all users
                if (searchText == "↺")
                {
                    return await query
                        .Select(u => new UserModel
                        {
                            UserId = u.UserId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Role = u.Role,
                            Email = u.Email
                        }).ToListAsync();
                }

                // If searchText is null or empty, return an empty list
                if (string.IsNullOrEmpty(searchText))
                {
                    return new List<UserModel>();
                }

                // Filter the user list based on the searchText
                var lowerSearchText = searchText.ToLower(); // Convert the search text to lowercase

                UserRole? roleEnum = Enum.TryParse(typeof(UserRole), searchText, true, out var parsedRole)
                            ? (UserRole?)parsedRole
                            : null;

                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearchText)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(lowerSearchText)) ||
                    (u.Email != null && u.Email.ToLower().Contains(lowerSearchText)) ||
                    (roleEnum != null && u.Role == roleEnum));

                // Select only the necessary fields for UserModel (excluding UserInt)
                return await query
                    .Select(u => new UserModel
                    {
                        UserId = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Role = u.Role,
                        Email = u.Email
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                // Log.Error(ex, "Error fetching users");
                throw new Exception("An error occurred while fetching users. Please try again later.", ex);
            }
        }


        public async Task<ResultStatusDto<UserModel>> UpdateUserAsync(UserModel userUpdateRequest, UserModel requestingUser, bool isSelfUpdate)
        {
            try
            {
                if (userUpdateRequest == null)
                {
                    throw new ArgumentNullException(nameof(userUpdateRequest), "User model is null.");
                }

                // Fetch the existing user from the database
                var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userUpdateRequest.UserId);
                if (dbUser == null)
                {
                    return ResultStatusDto<UserModel>.CreateError("User not found.");
                }

                // If the current user is a regular user, ensure they can only update their own profile
                if (requestingUser.UserId == dbUser.UserId && requestingUser.UserId == userUpdateRequest.UserId && isSelfUpdate)
                {
                    // Users can update their name, last name, and email but not role
                    dbUser.FirstName = userUpdateRequest.FirstName;
                    dbUser.LastName = userUpdateRequest.LastName;
                    dbUser.Email = userUpdateRequest.Email;
                    // Role remains unchanged for regular users
                }
                else if (requestingUser.Role == UserRole.Admin)
                {

                    if (dbUser.Role == UserRole.Admin || dbUser.Role == UserRole.SuperAdmin)
                    {
                        return ResultStatusDto<UserModel>.CreateError("Admins cannot change other Admin or SuperAdmin details.");
                    }

                    // Admins can update users, but can only assign roles up to 'User'
                    if (userUpdateRequest.Role == UserRole.Admin || userUpdateRequest.Role == UserRole.SuperAdmin)
                    {
                        return ResultStatusDto<UserModel>.CreateError("Admins cannot assign Admin or SuperAdmin roles.");
                    }

                    // Admins can update other properties, including role up to 'User'
                    dbUser.FirstName = userUpdateRequest.FirstName;
                    dbUser.LastName = userUpdateRequest.LastName;
                    dbUser.Email = userUpdateRequest.Email;
                    dbUser.Role = userUpdateRequest.Role; // Role can be updated, but limited to 'User'
                }
                else if (requestingUser.Role == UserRole.SuperAdmin)
                {

                    if (userUpdateRequest.Role == UserRole.SuperAdmin)
                    {
                        return ResultStatusDto<UserModel>.CreateError("You cannot change other SuperAdmin details.");
                    }

                    // SuperAdmins can assign any role up to 'Admin'
                    if (userUpdateRequest.Role == UserRole.SuperAdmin)
                    {
                        return ResultStatusDto<UserModel>.CreateError("You cannot assign SuperAdmin role.");
                    }

                    // SuperAdmins can update other properties, including role up to 'Admin'
                    dbUser.FirstName = userUpdateRequest.FirstName;
                    dbUser.LastName = userUpdateRequest.LastName;
                    dbUser.Email = userUpdateRequest.Email;
                    dbUser.Role = userUpdateRequest.Role; // Role can be updated, but limited to 'Admin'
                }
                else
                {
                    return ResultStatusDto<UserModel>.CreateError("Unauthorized operation.");
                }

                // Check if the email is already taken by another user
                var emailExists = await _context.Users
                    .Where(u => u.Email == userUpdateRequest.Email && u.UserId != userUpdateRequest.UserId) // Ensure it is not the same user
                    .AnyAsync();

                if (emailExists)
                {
                    return ResultStatusDto<UserModel>.CreateError("Email already exists for another user.");
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
                return ResultStatusDto<UserModel>.CreateSuccess("User updated successfully.", userUpdateRequest);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return ResultStatusDto<UserModel>.CreateError(ex.Message.ToString());
            }
        }


        public async Task<ResultStatusDto<NewUserModel>> AddUserAsync(NewUserModel newUser, UserModel? requestingUser = null)
        {
            try
            {
                if (newUser == null)
                {
                    return ResultStatusDto<NewUserModel>.CreateError("User is empty.");
                }

                // Check if the email is already in use
                var dbUser = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
                if (dbUser)
                {
                    return ResultStatusDto<NewUserModel>.CreateError("User email already exists in the database.");
                }

                // If there's no current user, it's a self-signup
                if (requestingUser == null)
                {
                    // Self-signup is only allowed for User or Guest roles
                    if (newUser.Role != UserRole.User && newUser.Role != UserRole.Guest)
                    {
                        return ResultStatusDto<NewUserModel>.CreateError("You can only sign up as a User or Guest.");
                    }
                }
                else
                {
                    // If current user is an Admin or SuperAdmin trying to add another user
                    if (requestingUser.Role == UserRole.Admin)
                    {
                        // Admins can add users up to Admin role
                        if (newUser.Role == UserRole.SuperAdmin || newUser.Role == UserRole.Admin)
                        {
                            return ResultStatusDto<NewUserModel>.CreateError("Admins cannot create Admins or SuperAdmins.");
                        }
                    }
                    else if (requestingUser.Role == UserRole.SuperAdmin)
                    {
                        if (newUser.Role == UserRole.SuperAdmin)
                        {
                            return ResultStatusDto<NewUserModel>.CreateError("SuperAdmin cannot create other SuperAdmins.");
                        }
                    }
                    else
                    {
                        // Other roles are not authorized to add users
                        return ResultStatusDto<NewUserModel>.CreateError("You are not authorized to add users.");
                    }
                }

                // Proceed to create the new user
                var mappedUser = _mapper.Map<EntityUserModel>(newUser);
                mappedUser.UserId = await GenerateUniqueUserIdAsync(); // Generate unique ID

                newUser.UserId = mappedUser.UserId; // Assign the generated ID to the model

                await _context.Users.AddAsync(mappedUser);
                await _context.SaveChangesAsync();

                return ResultStatusDto<NewUserModel>.CreateSuccess("User created successfully.", newUser);
            }
            catch (Exception ex)
            {
                // Handle any exception and return an error response
                return ResultStatusDto<NewUserModel>.CreateError(ex.Message);
            }
        }


        public async Task<ResultStatusDto<UserModel>> DeleteUserAsync(UserModel userToRemove, UserModel requestingUser, bool isSelfDelete)
        {
            try
            {
                // Check if the user to be deleted exists
                var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userToRemove.UserId);

                if (dbUser == null)
                {
                    return ResultStatusDto<UserModel>.CreateError("User does not exist.");
                }

                // If the current user is deleting themselves, allow it without further checks
                if ((requestingUser.UserId == userToRemove.UserId) && isSelfDelete)
                {
                    // Proceed to delete the user
                    _context.Users.Remove(dbUser);
                    await _context.SaveChangesAsync();
                    return ResultStatusDto<UserModel>.CreateSuccess("Your account has been deleted.");
                }
                if ((requestingUser.UserId == userToRemove.UserId) && !isSelfDelete)
                {
                    return ResultStatusDto<UserModel>.CreateError("You cannot delete your own account from here.");
                }
                // Role-based deletion checks for when current user is deleting someone else
                if (requestingUser.Role == UserRole.SuperAdmin)
                {
                    if (dbUser.Role == UserRole.SuperAdmin)
                    {
                        return ResultStatusDto<UserModel>.CreateError("SuperAdmin cannot delete other SuperAdmins.");
                    }
                }
                else if (requestingUser.Role == UserRole.Admin)
                {
                    // Admin can delete anyone except Admins and SuperAdmins
                    if (dbUser.Role == UserRole.Admin || dbUser.Role == UserRole.SuperAdmin)
                    {
                        return ResultStatusDto<UserModel>.CreateError("Admins cannot delete other Admins or SuperAdmins.");
                    }
                }
                else
                {
                    // If the current user is neither SuperAdmin nor Admin
                    return ResultStatusDto<UserModel>.CreateError("You are not authorized to delete users.");
                }

                // Proceed to delete the user
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();

                return ResultStatusDto<UserModel>.CreateSuccess("User deleted successfully.");
            }
            catch (Exception ex)
            {
                // Handle any exception and return an error response
                return ResultStatusDto<UserModel>.CreateError(ex.Message);
            }
        }



        public async Task<ResultStatusDto<UserModel>> SetPassword(UserModel model, string newPassword, string? oldPassword = null, string? otp = null)
        {
            if (!string.IsNullOrEmpty(oldPassword))
            {
                // Logic to handle old password updating goes here...
                return ResultStatusDto<UserModel>.CreateSuccess("Password updated successfully using old password.", model);
            }
            else if (!string.IsNullOrEmpty(otp))
            {
                // Logic to handle OTP verification goes here...
                return ResultStatusDto<UserModel>.CreateSuccess("Password updated successfully using OTP.", model);
            }
            else
            {
                // Generate a new salt and hash the new password
                var salt = _passwordManager.GenerateSalt();
                var hashedPassword = _passwordManager.HashPassword(newPassword, salt);

                // Find the existing user using UserId
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);

                if (existingUser == null)
                {
                    return ResultStatusDto<UserModel>.CreateError("User not found.");
                }

                // Search the UserCred table for existing credentials for this user
                var existingUserCred = await _context.UserCred.FirstOrDefaultAsync(uc => uc.UserInt == existingUser.UserInt);

                if (existingUserCred == null)
                {
                    // If no existing credentials, create a new UserCred record
                    var userCredModel = new EntityUserCredModel
                    {
                        UserInt = existingUser.UserInt,
                        HashedPassword = hashedPassword,
                        Salt = salt
                    };
                    await _context.UserCred.AddAsync(userCredModel);
                }
                else
                {
                    // If credentials exist, update the existing record
                    existingUserCred.HashedPassword = hashedPassword;
                    existingUserCred.Salt = salt;
                    _context.UserCred.Update(existingUserCred);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return ResultStatusDto<UserModel>.CreateSuccess("Password updated successfully.", model);
            }
        }

        private async Task<string> GenerateUniqueUserIdAsync()
        {
            string userId;
            bool exists;

            do
            {
                userId = GenerateRandomUserId();
                // Check if the generated UserId already exists
                exists = await _context.Users.AnyAsync(u => u.UserId == userId);
            } while (exists); // Repeat until a unique UserId is found

            return userId;
        }

        // Method to generate a random UserId in uppercase
        private string GenerateRandomUserId()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Allowed characters
            var userId = new char[10];

            for (int i = 0; i < userId.Length; i++)
            {
                userId[i] = chars[random.Next(chars.Length)];
            }

            return new string(userId); // Convert char array to string
        }
    }
}

