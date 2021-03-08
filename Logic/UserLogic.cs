using Model;
using Model.DataTransfer;
using Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public class UserLogic
    {
        private readonly Repo _repo;
        private readonly Mapper _mapper;

        public UserLogic(Repo repo, Mapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            List<UserDto> userDtos = new List<UserDto>();
            var users = await _repo.GetAllUsers();
            foreach(ApplicationUser u in users)
            {
                UserDto userDto = _mapper.ConvertUserToUserDto(u);
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            return _mapper.ConvertUserToUserDto(await _repo.GetUserById(id));
        }
    }
}
