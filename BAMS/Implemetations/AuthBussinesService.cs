using BAMS.Interface;
using DAMS.Interface;
using DAMS.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Implemetations
{
    public class AuthBussinesService:IAuthBussinesService
    {
        private readonly IAuthService _authService;
        public AuthBussinesService(IAuthService authService)
        {
            _authService = authService;
        }
    
        public async Task<AuthResponse> Authenticate(string username, string password)
        {
            return await _authService.Authenticate(username, password);
        }

        public void ResendOtp()
        {
            _authService.ResendOtp();
        }

        public bool VerifyOtp(string otp)
        {
            return _authService.VerifyOtp(otp);
        }
    }
}
