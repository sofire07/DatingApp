using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ILikeRepository
    {
        Task<bool> AddLike(UserLike like);
        Task<bool> RemoveLike(UserLike like);
        Task<UserLike> GetLikeByUsers(string userLiking, string userBeingLiked);
        Task<IEnumerable<UserDto>> GetLikesByUser(string username);
        Task<IEnumerable<UserDto>> GetLikedByUser(string username);
    }
}
