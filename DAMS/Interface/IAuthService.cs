using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAMS.Models.Auth;

namespace DAMS.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> Authenticate(string username, string password);
        bool VerifyOtp(string otp);
        void ResendOtp();

    }
}
