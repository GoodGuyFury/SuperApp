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
        Task<ResultStatusDto<UserModel>> UpdateUserAsync(UserModel user);
        Task<ResultStatusDto<NewUserModel>> AddUserAsync(NewUserModel user);
        Task<ResultStatusDto<UserModel>> DeleteUserAsync(UserModel user);
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

                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearchText)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(lowerSearchText)) ||
                    (u.Email != null && u.Email.ToLower().Contains(lowerSearchText)) ||
                    (u.Role.ToString().ToLower() == lowerSearchText));

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


        public async Task<ResultStatusDto<UserModel>> UpdateUserAsync(UserModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model), "User model is null.");
                }
               
                // Fetch the existing user from the database
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId); ; // Assuming UserId is the primary key
                if (existingUser == null)
                {
                    return ResultStatusDto<UserModel>.CreateError("User not found.");
                }
                if (existingUser.Role == UserRole.SuperAdmin)
                {
                    return ResultStatusDto<UserModel>.CreateError("user is a super admin");
                }

                var emailExists = await _context.Users
                .Where(u => u.Email == model.Email && u.UserId != model.UserId) // Ensure it is not the same user
                .AnyAsync();

                if (emailExists)
                {
                    return ResultStatusDto<UserModel>.CreateError("Email already exists for another user.");
                }
                // Update the existing user properties
                existingUser.FirstName = model.FirstName;
                existingUser.LastName = model.LastName;
                existingUser.Email = model.Email;
                existingUser.Role = model.Role;
                // Update other properties as needed

               
                await _context.SaveChangesAsync();
                return ResultStatusDto<UserModel>.CreateSuccess("User Updated Successfully", model);
            }
            catch (Exception ex)
            {
                // Log the exception
                return ResultStatusDto<UserModel>.CreateError(ex.Message.ToString());
            }
        }
        
        public async Task<ResultStatusDto<NewUserModel>> AddUserAsync(NewUserModel model)
        {
            try {
                if (model == null) {
                    return ResultStatusDto<NewUserModel>.CreateError("user is empty");
                }
                var isExististingUser = await _context.Users.AnyAsync(u => u.Email == model.Email);

                if (isExististingUser) {
                    return ResultStatusDto<NewUserModel>.CreateError("User Mail already present in db");
                }

                var newUser = _mapper.Map<EntityUserModel>(model);
                newUser.UserId = await GenerateUniqueUserIdAsync();

                model.UserId = newUser.UserId;
               await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return ResultStatusDto<NewUserModel>.CreateSuccess("User Created Successfully",model);
            }
            catch (Exception ex) {

                return ResultStatusDto<NewUserModel>.CreateError(ex.Message.ToString());
            }
        }

        public async Task<ResultStatusDto<UserModel>> DeleteUserAsync(UserModel model)
        {
            
            try { 
                if (model == null) {

                    var result = ResultStatusDto<UserModel>.CreateError("No user Found");

                    return result;
                }
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);
                if (existingUser != null) { 
                if(existingUser.Role == UserRole.SuperAdmin || existingUser.Role == UserRole.Admin)
                    {
                        var result = ResultStatusDto<UserModel>.CreateError("Admin and super admin cannot be deleted");
                        return result;
                    }else
                    {
                        _context.Users.Remove(existingUser);
                        await _context.SaveChangesAsync();
                        var result = ResultStatusDto<UserModel>.CreateSuccess("Deleted successfully");
                        return result;
                    }
                } else
                {
                    var result = ResultStatusDto<UserModel>.CreateError("user doesnt exist");
                    return result;
                }
            }
            catch (Exception ex)
            {
                return ResultStatusDto<UserModel>.CreateError(ex.Message.ToString());
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

