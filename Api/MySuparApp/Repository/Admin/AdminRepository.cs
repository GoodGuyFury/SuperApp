using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuthModel; // Assuming UserModel is defined here
using ApplicationDbContextShared; // Assuming ApplicationDbContext is defined here

namespace AdminRepository
{
    public class UserManagement
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
                return users.Where(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (u.LastName != null && u.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Email != null && u.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Role != null && u.Role.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                // Log.Error(ex, "Error fetching users");
                throw new Exception("An error occurred while fetching users. Please try again later.", ex);
            }
        }

        public async Task CreateUserAsync(UserModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model), "User model is null.");
                }

                await _context.Users.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while creating the user.", ex);
            }
        }
    }
}

