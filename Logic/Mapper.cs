using Model;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class Mapper
    {
        public UserDto ConvertUserToUserDto(ApplicationUser user)
        {
            UserDto convertedUser = new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
            };
            return convertedUser;
        }
    }
}
