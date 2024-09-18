namespace MySuparApp.Models.Authentication
{
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

    public class UserInfo
    {
        public string fullName { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }

    public class AuthenticationResult
    {
        public VerificationResultDto verificationResult { get; set; }
        public UserInfo userInfo { get; set; }

        public AuthenticationResult()
        {
            verificationResult = new VerificationResultDto();
            userInfo = new UserInfo();
        }
    }
}
