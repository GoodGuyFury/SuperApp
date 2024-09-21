using System.ComponentModel.DataAnnotations;

namespace AuthModel
{
  
    public class UserModel
    {
        [Key] public string UserId { get; set; } = string.Empty; // Primary key
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class GoogleUserInfo
    {
        public bool emailVerified { get; set; } = false;
        public string email { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string picture { get; set; } = string.Empty;
        public string msg { get; set; } = string.Empty;
    }

    public class VerificationResultDto
    {
        public string status { get; set; } = "error";
        public string message { get; set; } = "Failed to authenticate";
    }

    public class AuthenticationResult
    {
        public VerificationResultDto verificationResult { get; set; }
        public UserModel userInfo { get; set; }

        public AuthenticationResult()
        {
            verificationResult = new VerificationResultDto();
            userInfo = new UserModel();
        }
    }
}
