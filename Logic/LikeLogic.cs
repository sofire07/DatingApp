using AutoMapper;
using Model;
using Model.DataTransfer;
using Model.Helpers;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class LikeLogic : ILikeLogic
    {
        private readonly ILikeRepository _likeRepo;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        public LikeLogic(ILikeRepository likeRepo, IUserRepo userRepo, IMapper mapper)
        {
            _likeRepo = likeRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }
        public async Task<bool?> AddLike(string loggedIn, string beingLiked)
        {

            var loggedInUser = await _userRepo.GetUserByUsername(loggedIn);
            var beingLikedUser = await _userRepo.GetUserByUsername(beingLiked);
            UserLike like = await _likeRepo.GetLikeByUsers(loggedIn, beingLiked);
            if (like != null) return null;
            UserLike newLike = new UserLike {
                UserBeingLiked = beingLikedUser,
                UserBeingLikedId = beingLikedUser.Id,
                UserLiking = loggedInUser,
                UserLikingId = loggedInUser.Id
            };

            return await _likeRepo.AddLike(newLike);
        }

        public async Task<bool?> RemoveLike(string loggedIn, string beingLiked)
        {
            var like = await _likeRepo.GetLikeByUsers(loggedIn, beingLiked);
            if (like == null) return null;
            return await _likeRepo.RemoveLike(like);
        }

        public async Task<IEnumerable<UserDto>> GetLikedBy(string username)
        {
            return await _likeRepo.GetLikesByUser(username);
            
        }

        public async Task<IEnumerable<UserDto>> GetLiking(string username)
        {
            return await _likeRepo.GetLikedByUser(username);
        }
    }
}
