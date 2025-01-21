using BAMS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthBussinesService _authManagerService;

        public AuthController(IAuthBussinesService authManagerService)
        {
            _authManagerService = authManagerService;
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]

        public async Task<IActionResult> Authenticate(string username, string password)
        {
            var authResponse = await _authManagerService.Authenticate(username, password);

            if (authResponse == null)
            {
                return Unauthorized(new { message = "Authentication failed" });
            }

            return Ok(authResponse);
        }


        [HttpPost("ResendOtp")]
        public IActionResult ResendOtp()
        {
            try
            {
                _authManagerService.ResendOtp();
                return Ok(new { Message = "OTP has been resent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                return BadRequest(new { message = "OTP cannot be empty" });
            }

            var isOtpValid = _authManagerService.VerifyOtp(otp);

            if (!isOtpValid)
            {
                return Unauthorized(new { message = "Authentication failed" });
            }

            return Ok(new { message = "OTP verified successfully" });
        }

    }
}
