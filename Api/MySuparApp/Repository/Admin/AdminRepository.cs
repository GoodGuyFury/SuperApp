using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySuparApp.Models.Authentication; // Assuming UserModel is defined here
using MySuparApp.Shared; // Assuming ApplicationDbContext is defined here

namespace MySuparApp.Repository.Admin
{
   
    public interface IAdminRepository
    {
        Task<List<UserModel>> GetUsersAsync(string searchText);
        Task<UserModel> CreateUserAsync(UserModel model);
        Task<UserModel> UpdateUserAsync(UserModel model);
    }
    public class UserManagement : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public UserManagement(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserModel>> GetUsersAsync(string searchText)
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                // If searchText is "6699a98ll", return all users
                if (searchText == "6699a98ll")
                {
                    return users;
                }

                // If searchText is null or empty, return an empty list
                if (string.IsNullOrEmpty(searchText))
                {
                    return new List<UserModel>();
                }

                // Filter the user list based on the searchText
             
                    var query = _context.Users.AsQueryable();

                    if (!string.IsNullOrEmpty(searchText) && searchText != "6699a98ll")
                    {
                        var lowerSearchText = searchText.ToLower(); // Convert the search text to lowercase

                        query = query.Where(u =>
                            (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearchText)) ||
                            (u.LastName != null && u.LastName.ToLower().Contains(lowerSearchText)) ||
                            (u.Email != null && u.Email.ToLower().Contains(lowerSearchText)) ||
                            (u.Role != null && u.Role.ToLower().Contains(lowerSearchText)));
                    }

                    return await query.ToListAsync();
                
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                // Log.Error(ex, "Error fetching users");
                throw new Exception("An error occurred while fetching users. Please try again later.", ex);
            }
        }

        public async Task<UserModel> CreateUserAsync(UserModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model), "User model is null.");
                }

                // Optionally, check if the user already exists (if there's a unique field like email or username)
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email); // assuming Email is a unique field
                if (existingUser != null)
                {
                    throw new Exception("A user with the same email already exists.");
                }

                await _context.Users.AddAsync(model);
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while creating the user.", ex);
            }
        }

        public async Task<UserModel> UpdateUserAsync(UserModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model), "User model is null.");
                }

                // Fetch the existing user from the database
                var existingUser = await _context.Users.FindAsync(model.UserId); // Assuming UserId is the primary key
                if (existingUser == null)
                {
                    throw new Exception("User not found.");
                }

                // Update the existing user properties
                existingUser.FirstName = model.FirstName;
                existingUser.LastName = model.LastName;
                existingUser.Email = model.Email;
                existingUser.Role = model.Role;
                // Update other properties as needed

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }
    }
}

