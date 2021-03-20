using Model;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface IUserLogic
    {

        Task<List<UserDto>> GetAllUsers();

        Task<UserDto> GetUserById(string id);

        Task<UserDto> GetUserByUsername(string username);

        Task<bool> EditUser(UserLoggedInDto updatedUser);

        Task SeedDatabase();
    }
}
