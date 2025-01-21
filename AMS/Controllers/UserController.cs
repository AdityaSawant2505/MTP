using BAMS.Interface;
using DAMS.Models.User;
using DAMS.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBussinesService _usersManagerService;
        private readonly PasswordHelper _passwordHelper;
        public UserController(IUserBussinesService usersManagerService, PasswordHelper passwordHelper)
        {
            _usersManagerService = usersManagerService;
            _passwordHelper = passwordHelper;
        }


        [HttpGet("GetUsersByName")]

        public async Task<IActionResult> GetUsersByName(string UsersName)
        {
            var result = await _usersManagerService.GetUserByName(UsersName);
            return Ok(result);
        }

        [HttpGet("GetPermissions")]

        public async Task<IActionResult> GetPermissions(long userid)
        {
            var result = await _usersManagerService.GetPermissions(userid);
            return Ok(result);
        }


        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles(long userid)
        {
            var result = await _usersManagerService.GetRoles(userid);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] InsertUserRequest request)
        {
            if (request == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                // Hash password
                var (hashedPassword, salt) = _passwordHelper.HashPassword(request.Password);

                // Set the password hash and salt in the request
                request.PasswordHash = hashedPassword;
                request.PasswordSalt = salt;

                // Insert user data into the database using UsersService (stored procedure)
                var result = await _usersManagerService.CreateUser(request);

                if (result)
                {
                    return Ok("User registered successfully.");
                }
                else
                {
                    return StatusCode(500, "An error occurred while registering the user.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
