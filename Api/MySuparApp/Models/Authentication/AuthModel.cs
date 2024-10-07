using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MySuparApp.Models.Authentication
{

    public enum UserRole
    {
        Admin = 1,
        SuperAdmin = 2,
        User = 3,
        Guest = 4,
        Mods = 5// Add other roles as needed
    }
    public class UserModel
    {
        

        [Required]
        [MaxLength(10)]
        public string UserId { get; set; } = string.Empty; // Unique key

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
   
    public class NewUserModel
    {
        [MaxLength(10)]
        public string? UserId { get; set; } = string.Empty; // Unique key (optional during creation)

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }

        [Required]
        [EmailAddress]
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
    public class CurrentUser :UserModel
    {

    }
    public class VerificationResultDto
    {
        public string Status { get; } = "unauthorized";
        public string Message { get; } = "Failed to authenticate";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserModel ? UserInfo { get; } // Add a property to hold user information

        // Private constructor to prevent direct instantiation
        private VerificationResultDto(string status, string message, UserModel? userInfo = null)
        {
            Status = status;
            Message = message;
            UserInfo = userInfo;
        }

        // Static factory methods
        public static VerificationResultDto CreateError(string message = "Failed to authenticate")
        {
            return new VerificationResultDto("unauthorized", message);
        }

        public static VerificationResultDto CreateSuccess(UserModel userInfo, string message = "Authentication successful")
        {
            return new VerificationResultDto("authorized", message, userInfo);
        }
        public string GetStatus() => Status;
        public string GetMessage() => Message;
    }

    public class ResultStatusDto<T>
    {
        public string Status { get; } // Status is read-only
        public string Message { get; } // Message is read-only
        public T? Data { get; } // Generic object of any type, optional

        // Constructor for creating a result with custom values
        private ResultStatusDto(string status, string message, T? data = default)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        // Static factory method for error creation
        public static ResultStatusDto<T> CreateError(string message = "Failed")
        {
            return new ResultStatusDto<T>("error", message);
        }

        // Static factory method for success, with an optional generic data
        public static ResultStatusDto<T> CreateSuccess(string message = "Success", T? data = default)
        {
            return new ResultStatusDto<T>("success", message, data);
        }

        // Getter methods
        public string GetStatus() => Status;
        public string GetMessage() => Message;
        public T? GetData() => Data;
    }



}
