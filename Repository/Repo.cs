using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class Repo
    {
        private readonly ApplicationDbContext _context;

        public Repo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            return await _context.AppUsers.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            return await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == username);
        }
    }
}
