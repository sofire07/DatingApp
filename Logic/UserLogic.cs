using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DataTransfer;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepo _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserLogic(IUserRepo repo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            List<UserDto> userDtos = new List<UserDto>();
            var users = await _repo.GetAllUsers();
            foreach(ApplicationUser u in users)
            {
                UserDto userDto = _mapper.Map<UserDto>(u);
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            return _mapper.Map<UserDto>(await _repo.GetUserById(id));
        }

        public async Task<UserDto> GetUserByUsername(string username)
        {
            return _mapper.Map<UserDto>(await _repo.GetUserByUsername(username));
        }

        public async Task<bool> EditUser(UserLoggedInDto updatedUser)
        {
            var user = await _repo.GetUserById(updatedUser.Id);
            if (!String.IsNullOrEmpty(updatedUser.City) && user.City != updatedUser.City) user.City = updatedUser.City;
            if (!String.IsNullOrEmpty(updatedUser.State) && user.State != updatedUser.State) user.State = updatedUser.State;
            if (!String.IsNullOrEmpty(updatedUser.Interests) && user.Interests != updatedUser.Interests) user.Interests = updatedUser.Interests;
            if (!String.IsNullOrEmpty(updatedUser.Introduction) && user.Introduction != updatedUser.Introduction) user.Introduction = updatedUser.Introduction;
            if (!String.IsNullOrEmpty(updatedUser.KnownAs) && user.KnownAs != updatedUser.KnownAs) user.KnownAs = updatedUser.KnownAs;
            if (!String.IsNullOrEmpty(updatedUser.LookingFor) && user.LookingFor != updatedUser.LookingFor) user.LookingFor = updatedUser.LookingFor;
            if (!String.IsNullOrEmpty(updatedUser.PhoneNumber) && user.PhoneNumber != updatedUser.PhoneNumber) user.PhoneNumber = updatedUser.PhoneNumber;
            if (!String.IsNullOrEmpty(updatedUser.Email) && user.Email != updatedUser.Email) user.Email = updatedUser.Email;
            await _repo.EditUser(user);
            return await _repo.SaveAllAsync();
        }

        public async Task SeedDatabase()
        {
            var userData = await System.IO.File.ReadAllTextAsync("C:/Users/csoph/Desktop/Revature/Practice/DatingApp/DatingApp/Repository/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<ApplicationUser>>(userData);
            foreach (var user in users)
            {
                var result = await _userManager.CreateAsync(user, "Password123!");
            }
        }
    }
}
