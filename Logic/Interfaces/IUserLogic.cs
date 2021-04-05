using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface IUserLogic
    {

        Task<PagedList<UserDto>> GetAllUsers(UserParams userParams, string username);

        Task<UserDto> GetUserById(string id);

        Task<UserDto> GetUserByUsername(string username);

        Task<bool> EditUser(UserLoggedInDto updatedUser);
    }
}
