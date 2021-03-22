using Microsoft.AspNetCore.Http;
using Model;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface IPhotoLogic
    {
        Task<PhotoDto> AddPhoto(UserDto loggedIn, IFormFile file);
        Task<bool> UpdateMainPhoto(UserDto loggedIn, int photoId);
        Task<bool?> DeletePhoto(UserDto loggedIn, int photoId);
    }
}
