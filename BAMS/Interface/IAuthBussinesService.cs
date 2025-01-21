using DAMS.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Interface
{
    public interface IAuthBussinesService
    {
        Task<AuthResponse> Authenticate(string username, string password);
        bool VerifyOtp(string otp);

        void ResendOtp();

    }
}
