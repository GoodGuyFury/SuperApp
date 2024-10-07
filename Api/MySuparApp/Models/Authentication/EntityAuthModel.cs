using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MySuparApp.Models.Authentication
{
  
        public class EntityUserModel : UserModel
        {
            [Key] // Primary Key
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
            public int UserInt { get; set; }  // Primary key (auto-generated)
        public virtual ICollection<EntityUserCredModel> UserCreds { get; set; } = new List<EntityUserCredModel>();
    }

        public class EntityUserCredModel
        {
            [Key] public int UserInt { get; set; }
            public string HashedPassword { get; set; } = string.Empty;
            public string Salt { get; set; } = string.Empty;

        public virtual EntityUserModel User { get; set; }
    }
   
}
