using AutoMapper;
using Microsoft.AspNetCore.Http;
using Model;
using Model.DataTransfer;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class PhotoLogic : IPhotoLogic
    {
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        private readonly IPhotoService _photoService;

        public PhotoLogic(IMapper mapper, IUserRepo userRepo, IPhotoService photoService)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _photoService = photoService;
        }

        public async Task<PhotoDto> AddPhoto(UserDto loggedIn, IFormFile file)
        {
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return null;
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (loggedIn.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            var user = await _userRepo.GetUserById(loggedIn.Id);
            user.Photos.Add(photo);
            await _userRepo.EditUser(user);
            if (await _userRepo.SaveAllAsync())
            {
                return _mapper.Map<PhotoDto>(photo);
            }
            return null;
        }

        public async Task<bool> UpdateMainPhoto(UserDto loggedIn, int photoId)
        {
            var user = await _userRepo.GetUserById(loggedIn.Id);
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.IsMain) return false;

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            //await _userRepo.EditUser(user);
            if (await _userRepo.SaveAllAsync()) return true;
            return false;
        }

        public async Task<bool?> DeletePhoto(UserDto loggedIn, int photoId)
        {
            var user = await _userRepo.GetUserById(loggedIn.Id);
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return null;
            if (photo.IsMain) return false;
            if (photo.ApplicationUserId != loggedIn.Id) return false;
            if (photo.PublicId != null)
            {
               var result = await _photoService.DeletePhotoAsync(photo.PublicId);
               if (result.Error != null) return false;
            }
            await _userRepo.DeletePhoto(photo);
            await _userRepo.EditUser(user);
            await _userRepo.SaveAllAsync();
            return true;
        }

    }
}
