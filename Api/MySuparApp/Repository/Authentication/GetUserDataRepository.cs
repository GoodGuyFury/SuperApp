using MySuparApp.Shared;
using MySuparApp.Models.Authentication;
using System.Drawing;
using System.Security.Cryptography;
using MySuperApp.Repository.Authentication;

namespace MySuparApp.Repository.Authentication
{
    public interface IGetUserData
    {
        Task<UserModel> GetUserDetails(string email, string pass = "", bool authToken = false);
    }

    public class GetUserData : IGetUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordManager _passwordmanager;
        public GetUserData(ApplicationDbContext context, IPasswordManager passwordmanager)
        {
            _context = context;
            _passwordmanager = passwordmanager;
        }

        public async Task<UserModel> GetUserDetails(string email, string pass = "", bool authToken = false)
        {
            try
            {
                var userDetails = await Task.Run(() =>
                {
                    return _context.Users
                        .Where(u => u.Email == email)
                        .Select(u => new
                        {
                            u.UserId,
                            u.FirstName,
                            u.LastName,
                            u.Role,
                            u.Email,
                            u.UserInt,
                            Credentials = _context.UserCred
                                .Where(cred => cred.UserInt == u.UserInt)
                                .Select(cred => new
                                {
                                    cred.HashedPassword,
                                    cred.Salt
                                })
                                .FirstOrDefault()
                        })
                        .FirstOrDefault();
                });

                if (userDetails == null)
                {
                    // User not found
                   throw new UserNotFoundException("User details is empty");
                }

                if (authToken)
                {
                    // Return user without checking credentials if authToken is true
                    return new UserModel
                    {
                        UserId = userDetails.UserId,
                        FirstName = userDetails.FirstName,
                        LastName = userDetails.LastName,
                        Role = userDetails.Role,
                        Email = userDetails.Email
                    };
                }

                // Verify password if credentials are provided
                if (userDetails.Credentials != null &&
                    _passwordmanager.VerifyPassword(pass, userDetails.Credentials.HashedPassword, userDetails.Credentials.Salt))
                {
                    return new UserModel
                    {
                        UserId = userDetails.UserId,
                        FirstName = userDetails.FirstName,
                        LastName = userDetails.LastName,
                        Role = userDetails.Role,
                        Email = userDetails.Email
                    };
                }

                // Invalid password
                throw new Exception("user password didnt match");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                throw new Exception($"An error occurred while fetching user details for User: {email}", ex);
            }
        }
    }
}
