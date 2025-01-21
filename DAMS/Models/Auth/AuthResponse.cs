using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Models.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Expires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}
