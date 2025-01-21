using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Models.User
{
    public class InsertUserRequest
    {
        public string UserHash { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }
        public long? ManagerId { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public string RefreshToken { get; set; }
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }
        public DateTime OtpCreateTime { get; set; }
    }
}
