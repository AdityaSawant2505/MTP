using DAMS.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using DAMS.Models.Auth;
using System.Threading.Tasks;
using DAMS.Models;
using DAMS.Models.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;



namespace DAMS.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _usersService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserService usersService, IConfiguration configuration, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _usersService = usersService;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<AuthResponse> Authenticate(string username, string password)
        {
            try
            {
                // Fetch user details from UsersService
                var user = await _usersService.GetUserByName(username);
                if (user == null || user.IsDeleted || username != user.UserName)
                    return null;

                // Verify password
                bool isPasswordValid = CheckPassword(user.PasswordHash, user.PasswordSalt, password);
                if (!isPasswordValid)
                    return null;

                // Fetch roles and permissions
                var roles = await _usersService.GetRoles(user.UserId);
                var permissions = await _usersService.GetPermissions(user.UserId);

                // Generate JWT token
                var authResponse = GenerateTokenResponse(user, roles, permissions);

                if (authResponse != null)
                {
                    var tokenBytes = Encoding.UTF8.GetBytes(authResponse.Token);
                    var session = _httpContextAccessor.HttpContext.Session;

                    if (session != null)
                    {
                        session.Set("AuthToken", tokenBytes); // Store the token in session
                        Console.WriteLine("Token stored in session successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Session is null.");
                    }

                    SendOtp(user.UserEmail,user.UserName);
                }
                return authResponse;
            }
            catch (Exception ex)
            {
                // Log any errors and rethrow
                Console.Error.WriteLine($"Error in {nameof(Authenticate)}: {ex.Message}");
                throw;
            }
        }

        private bool CheckPassword(string hash, string salt, string password)
        {
            var hashKey = Convert.FromBase64String(hash);
            var saltKey = Convert.FromBase64String(salt);

            using (var algorithm = new Rfc2898DeriveBytes(password, saltKey, 10000, HashAlgorithmName.SHA512))
            {
                var keyToCheck = algorithm.GetBytes(64);
                return keyToCheck.SequenceEqual(hashKey);
            }
        }

        public async Task<bool> RegisterUser(InsertUserRequest request)
        {
            // Hash password
            var (hashedPassword, salt) = HashPassword(request.Password);

            // Set password hash and salt
            request.PasswordHash = hashedPassword;
            request.PasswordSalt = salt;

            // Call UsersService to insert the user into the database
            var result = await _usersService.CreateUser(request);
            return result;
        }

        public (string hashedPassword, string salt) HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[16]; // 128-bit salt
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA512))
                {
                    var hashedPassword = pbkdf2.GetBytes(64);
                    return (Convert.ToBase64String(hashedPassword), Convert.ToBase64String(salt));
                }
            }
        }

        // Generate JWT token with roles and permissions
        private AuthResponse GenerateTokenResponse(GetUsers user, List<string> roles, List<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                CreatedOn = DateTime.UtcNow,
                Expires = tokenDescriptor.Expires,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpires = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"]))
            };
        }

        // Generate Refresh Token
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async void SendOtp(string email, string username)
        {
            List<string> recvrs = new List<string> { email };
            string subject = "OTP Verification";
            string otp = GenerateOtp();
            string template;

            try
            {
                using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("MyConn")))
                {
                    // Fetch email template from the database
                    var templateQuery = "SELECT Template FROM HtmlTemplates WHERE Name = 'OtpVerification'";
                    template = await sqlConnection.QueryFirstOrDefaultAsync<string>(templateQuery);

                    if (string.IsNullOrEmpty(template))
                    {
                        throw new Exception("Email template for OTP Verification not found.");
                    }

                    // Replace placeholders in the template with actual values
                    template = template.Replace("${username}", username)
                                       .Replace("${otp}", otp);

                    // Update OTP in the database
                    var parameters = new DynamicParameters();
                    parameters.Add("@username", username);
                    parameters.Add("@otp", otp);

                    await sqlConnection.OpenAsync();
                    await sqlConnection.ExecuteAsync(
                        "UpdateOtp", // Assuming this stored procedure updates OTP
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);
                    await sqlConnection.CloseAsync();
                }

                // Send the email
                await _emailService.SendEmailAsync(recvrs, subject, template, null);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error in SendOtp: {ex.Message}");
                throw;
            }
        }

        public string GenerateOtp()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        public void ResendOtp()
        {
            try
            {
                // Get the JWT token from the session
                var token = GetTokenFromSession();
                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("Token is missing from the session.");
                }

                // Parse the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                {
                    throw new InvalidOperationException("Invalid token.");
                }

                // Extract claims
                var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrEmpty(userNameClaim) || string.IsNullOrEmpty(emailClaim))
                {
                    throw new InvalidOperationException("Required claims are missing in the token.");
                }

                // Send OTP
                SendOtp(emailClaim, userNameClaim);
            }
            catch (Exception ex)
            {

                throw; // Rethrow to preserve stack trace
            }
        }


        public bool VerifyOtp(string otp)
        {
            var token = GetTokenFromSession();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token not found in session.");
                return false;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null)
            {
                Console.WriteLine("Invalid token.");
                return false;
            }

            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");
            if (userNameClaim == null)
            {
                Console.WriteLine("UserName claim not found in token.");
                return false;
            }

            string userName = userNameClaim.Value; 

            try
            {
                using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("MyConn")))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@username", userName);
                    parameters.Add("@otp", otp);
                    parameters.Add("@IsValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                    sqlConnection.Open();
                    sqlConnection.Execute("VerifyOtp", parameters, commandType: CommandType.StoredProcedure);

                    var isValid = parameters.Get<bool>("@IsValid");

                    Console.WriteLine($"Stored Procedure result: {isValid}");

                    return isValid; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying OTP: {ex.Message}");
                throw;
            }
        }

        public string GetTokenFromSession()
        {
            var session = _httpContextAccessor.HttpContext.Session;

            if (session != null)
            {
                // Retrieve the byte array from session using the custom Get extension method
                byte[] tokenBase64 = DAMS.Models.SessionExtensions.Get(session, "AuthToken");

                if (tokenBase64 != null && tokenBase64.Length > 0)
                {
                    // Convert the byte array to a string using UTF8 encoding
                    string token = Encoding.UTF8.GetString(tokenBase64);
                    return token;
                }
                else
                {
                    Console.WriteLine("AuthToken is not found in session.");
                }
            }
            else
            {
                Console.WriteLine("Session is null.");
            }
            return null;
        }
    }
}
