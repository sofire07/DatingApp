using Model;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return convertedUser;
        }

        public UserLoggedInDto ConvertUserToUserLoggedInDto(ApplicationUser user, JwtSecurityToken token)
        {
            UserLoggedInDto convertedUser = new UserLoggedInDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpires = token.ValidTo
            };
            return convertedUser;
        }
    }
}
