using System.Security.Cryptography;

namespace MySuperApp.Repository.Authentication
{
    public interface IPasswordManager
    {
        string GenerateSalt(int size = 32);
        string HashPassword(string password, string salt);
        bool VerifyPassword(string plainTextPassword, string hashedPassword, string storedSalt);
    }

    public class PasswordManager : IPasswordManager
    {
        public string GenerateSalt(int size = 32)
        {
            // Generate a cryptographic random number for the salt
            var saltBytes = new byte[size];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Convert the byte array to a Base64 string for storage
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            // Decode the salt back to a byte array
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var hmac = new HMACSHA256(saltBytes))
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hashBytes = hmac.ComputeHash(passwordBytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string plainTextPassword, string hashedPassword, string storedSalt)
        {
            // Hash the input password with the stored salt
            var hashedInputPassword = HashPassword(plainTextPassword, storedSalt);

            // Compare the hashes
            return hashedInputPassword == hashedPassword;
        }
    }
}
