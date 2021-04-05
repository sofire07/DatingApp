using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface ILikeLogic
    {
        Task<bool?> AddLike(string loggedIn, string beingLiked);
        Task<bool?> RemoveLike(string loggedIn, string beingLiked);

        Task<IEnumerable<UserDto>> GetLikedBy(string username);

        Task<IEnumerable<UserDto>> GetLiking(string username);


    }
}
