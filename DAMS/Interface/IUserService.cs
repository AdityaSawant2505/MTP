using DAMS.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Interface
{
    public interface IUserService
    {
        Task<bool> CreateUser(InsertUserRequest request);
        Task<GetUsers> GetUserByName(string name);
        Task<List<string>> GetPermissions(long userId);
        Task<List<string>> GetRoles(long userId);
    }
}
