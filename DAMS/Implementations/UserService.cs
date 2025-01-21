using DAMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using DAMS.Models.User;

namespace DAMS.Implementations
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            // Initialize the connection string from appsettings.json
            _connectionString = configuration.GetConnectionString("Myconn");
        }

        public async Task<GetUsers> GetUserByName(string name)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@userName", name);

                    var query = @"
                    SELECT 
                        user_id AS UserId,
                        user_hash AS UserHash,
                        user_name AS UserName,
                        user_email AS UserEmail,
                        department AS Department,
                        designation AS Designation,
                        manager_id AS ManagerId,
                        password_hash AS PasswordHash,
                        password_salt AS PasswordSalt,
                        is_deleted AS IsDeleted,
                        created_by AS CreatedBy,
                        created_on AS CreatedOn,
                        updated_by AS UpdatedBy,
                        updated_on AS UpdatedOn,
                        refresh_token AS RefreshToken,
                        reset_token AS ResetToken,
                        reset_token_expiry AS ResetTokenExpiry,
                        phone_number AS PhoneNumber,
                        otp AS Otp,
                        otp_create_time AS OtpCreateTime
                    FROM Users
                    WHERE user_name = @userName";

                    await sqlConnection.OpenAsync();

                    return await sqlConnection.QueryFirstOrDefaultAsync<GetUsers>(
                        query,
                        parameters,
                        commandType: System.Data.CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in {nameof(GetUserByName)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetPermissions(long userId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@userId", userId);

                    await sqlConnection.OpenAsync();

                    // Fetch permissions using stored procedure
                    return (await sqlConnection.QueryAsync<string>(
                        "GetPermissionsByUserId",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in {nameof(GetPermissions)}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetRoles(long userId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@userId", userId);

                    await sqlConnection.OpenAsync();

                    // Fetch roles using stored procedure
                    return (await sqlConnection.QueryAsync<string>(
                        "GetRolesByUserId",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in {nameof(GetRoles)}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CreateUser(InsertUserRequest request)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@userHash", request.UserHash);
                    parameters.Add("@userName", request.UserName);
                    parameters.Add("@userEmail", request.UserEmail);
                    parameters.Add("@department", request.Department);
                    parameters.Add("@designation", request.Designation);
                    parameters.Add("@managerId", request.ManagerId);
                    parameters.Add("@passwordHash", request.PasswordHash);
                    parameters.Add("@passwordSalt", request.PasswordSalt);
                    parameters.Add("@isDeleted", request.IsDeleted);
                    parameters.Add("@createdBy", request.CreatedBy);
                    parameters.Add("@createdOn", DateTime.UtcNow);
                    parameters.Add("@updatedBy", request.UpdatedBy);
                    parameters.Add("@updatedOn", DateTime.UtcNow);
                    parameters.Add("@refreshToken", request.RefreshToken);
                    parameters.Add("@resetToken", request.ResetToken);
                    parameters.Add("@resetTokenExpiry", request.ResetTokenExpiry);
                    parameters.Add("@phoneNumber", request.PhoneNumber);
                    parameters.Add("@Otp", request.Otp);
                    parameters.Add("@otpCreateTime", request.OtpCreateTime);

                    // Use stored procedure to create the user
                    var result = await sqlConnection.ExecuteAsync(
                        "CreateUser",
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure);

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in {nameof(CreateUser)}: {ex.Message}");
                throw;
            }
        }
    }
}
