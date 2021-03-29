using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DataTransfer;
using Model.Helpers;
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

        public async Task<PagedList<UserDto>> GetAllUsers(UserParams userParams, string username)
        {
            var user = await _repo.GetUserByUsername(username);
            userParams.CurrentUsername = username;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            return await _repo.GetUserDtos(userParams);

        }

        public async Task<UserDto> GetUserById(string id)
        {
            return _mapper.Map<UserDto>(await _repo.GetUserById(id));
        }

        public async Task<UserDto> GetUserByUsername(string username)
        {
            return await _repo.GetUserDto(username);
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

    }
}
