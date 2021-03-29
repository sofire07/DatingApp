using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRepo(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            return await _context.AppUsers.Include(x => x.Photos).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _context.AppUsers.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            return await _context.AppUsers.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<PagedList<UserDto>> GetUserDtos(UserParams userParams)
        {
            var query = _context.AppUsers.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DoB >= minDob && u.DoB <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<UserDto>.CreateAsync(query.ProjectTo<UserDto>(_mapper.ConfigurationProvider).AsNoTracking(), 
                userParams.PageNumber, userParams.PageSize);
        }

        public async Task<UserDto> GetUserDto(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task EditUser(ApplicationUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<bool> SaveAllAsync(){
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task DeletePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }

    }
}
