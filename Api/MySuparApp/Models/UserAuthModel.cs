namespace UserAuthModel
{
    public class GoogleUserInfo
    {
     public  bool EmailVerified { get; set; } = false;
     public  string Email { get; set; }  =string.Empty;
        public string msg { get; set; } = string.Empty; 
    }
    public class VerificationResultDto
    {
        public  string Status { get; set; } = "error";
        public  string Message { get; set; }= "Failed to authenticate";

    }
    public class UserInfo
    {
        public  string FullName { get; set; } = string.Empty;
        public  string Role { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

