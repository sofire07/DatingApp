using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LikeRepository:ILikeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LikeRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<bool> AddLike(UserLike like)
        {
            var result = _context.Likes.AddAsync(like);
            if (result.IsCompletedSuccessfully)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<UserLike> GetLikeByUsers(string userLiking, string userBeingLiked)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.UserLiking.UserName == userLiking && x.UserBeingLiked.UserName == userBeingLiked);
        }

        public async Task<IEnumerable<UserDto>> GetLikedByUser(string username)
        {
            //var likeList = await _context.Likes.Where(x => x.UserLiking.UserName == likeParams.CurrentUsername).Include(x => x.UserBeingLiked).Include(x=>x.UserBeingLiked.Photos).ToListAsync();
            var users = _context.AppUsers.Include(x=>x.Photos).AsQueryable();
            var likes = _context.Likes.OrderByDescending(x=>x.DateLiked).AsQueryable();

            likes = likes.Where(x => x.UserLiking.UserName == username);
            users = likes.Select(x => x.UserBeingLiked);


            return await users.ProjectTo<UserDto>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> GetLikesByUser(string username)
        {
            //var likeList= await _context.Likes.Where(x => x.UserBeingLiked.UserName == likeParams.CurrentUsername).Include(x=> x.UserLiking).Include(x => x.UserLiking.Photos).ToListAsync();
            var users = _context.AppUsers.Include(x => x.Photos).AsQueryable();
            var likes = _context.Likes.OrderByDescending(x => x.DateLiked).AsQueryable();

            likes = likes.Where(x => x.UserBeingLiked.UserName == username);
            users = likes.Select(x => x.UserLiking);

            return await users.ProjectTo<UserDto>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
        }

        public async Task<bool> RemoveLike(UserLike like)
        {
            try
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
