using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUserRepo
    {
        Task<List<ApplicationUser>> GetAllUsers();

        Task<ApplicationUser> GetUserById(string id);

        Task<ApplicationUser> GetUserByUsername(string username);
        Task<PagedList<UserDto>> GetUserDtos(UserParams userParams);

        Task<UserDto> GetUserDto(string username);

        Task EditUser(ApplicationUser user);

        Task<bool> SaveAllAsync();

        Task DeletePhoto(Photo photo);
    }
}
