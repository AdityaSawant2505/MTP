using BAMS.Interface;
using DAMS.Interface;
using DAMS.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Implemetations
{
    public class UserBussinesService:IUserBussinesService
    {
        private readonly IUserService _userService;
        public UserBussinesService(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<bool> CreateUser(InsertUserRequest request)
        {
            return await _userService.CreateUser(request);
        }

        public async Task<List<string>> GetPermissions(long userId)
        {
            return await _userService.GetPermissions(userId);
        }

        public async Task<List<string>> GetRoles(long userId)
        {
            return await _userService.GetRoles(userId);
        }

        public async Task<GetUsers> GetUserByName(string name)
        {
            return await _userService.GetUserByName(name);
        }
    }
}
