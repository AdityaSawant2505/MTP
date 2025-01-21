using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Models.User
{
    public class GetUsers
    {
        public long UserId { get; set; } 
        public string UserHash { get; set; } 
        public string UserName { get; set; } 
        public string UserEmail { get; set; } 
        public string Department { get; set; } 
        public string Designation { get; set; } 
        public long? ManagerId { get; set; } 
        public string PasswordHash { get; set; } 
        public string PasswordSalt { get; set; } 
        public bool IsDeleted { get; set; } // Maps to is_deleted
        public long CreatedBy { get; set; } // Maps to created_by
        public DateTime CreatedOn { get; set; } // Maps to created_on
        public long UpdatedBy { get; set; } // Maps to updated_by
        public DateTime UpdatedOn { get; set; } // Maps to updated_on
        public string RefreshToken { get; set; } // Maps to refresh_token
        public string ResetToken { get; set; } // Maps to reset_token
        public DateTime? ResetTokenExpiry { get; set; } // Maps to reset_token_expiry
        public string PhoneNumber { get; set; } // Maps to phone_number
        public string Otp { get; set; } // Maps to Otp
        public DateTime? OtpCreateTime { get; set; }
    }
}
