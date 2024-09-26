using MySuparApp.Shared;
using MySuparApp.Models.Authentication;

namespace MySuparApp.Repository.Authentication
{
    public interface IGetUserData
    {
        Task<UserModel> GetUserDetails(string email, string pass = "", bool authToken = false);
        Task<UserModel> InsertUser(string firstName, string lastName, string email, string password);
    }

    public class GetUserData : IGetUserData
    {
        private readonly ApplicationDbContext _context;

        public GetUserData(ApplicationDbContext context)
        {
            _context = context;
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
                            Credentials = _context.UserCred
                                .Where(cred => cred.UserId == u.UserId)
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
                    VerifyPassword(pass, userDetails.Credentials.HashedPassword, userDetails.Credentials.Salt))
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

        private bool VerifyPassword(string plainTextPassword, string hashedPassword, string salt)
        {
            // Combine the plain password with the stored salt
            var saltedPassword = plainTextPassword + salt;

            // Hash the salted password (without generating a new salt)
            var hashedInputPassword = HashPasswordWithoutNewSalt(saltedPassword);

            // Compare the hashes
            return hashedInputPassword == hashedPassword;
        }

        private string HashPasswordWithoutNewSalt(string input)
        {
            // Use a more deterministic hashing method like SHA-256 for password comparison
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        public async Task<UserModel> InsertUser(string firstName, string lastName, string email, string password)
        {
            try
            {
                // Generate a salt
                var salt = GenerateSalt();

                // Hash the password with the salt
                var hashedPassword = HashPasswordWithoutNewSalt(password + salt);

                // Create a new user entity
                var user = new UserModel
                {
                    UserId = Guid.NewGuid().ToString().Substring(0, 6), // Unique UserId
                    FirstName = firstName,
                    LastName = lastName,
                    Role = "user", // Example role
                    Email = email
                };

                // Add user to the database
               await _context.Users.AddAsync(user);

                // Create and add the credentials
                var credentials = new UserCredModel
                {
                    UserId = user.UserId,
                    HashedPassword = hashedPassword,
                    Salt = salt
                };

               await _context.UserCred.AddAsync(credentials);

                // Save changes to the database
               await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the insert operation
                throw new Exception("An error occurred while inserting the user.", ex);
            }
        }

        private string GenerateSalt()
        {
            // Generate a random salt (you can also use a fixed salt length)
            return Guid.NewGuid().ToString();
        }
    }
}
