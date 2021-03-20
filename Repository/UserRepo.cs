﻿using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<bool> EditUser(ApplicationUser user)
        {
            _context.AppUsers.Update(user);
            return await SaveAllAsync();
        }

        public async Task<bool> SaveAllAsync(){
            return await _context.SaveChangesAsync() > 0; 
        }

    }
}
