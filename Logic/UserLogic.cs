using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public class UserLogic
    {
        private readonly Repo _repo;

        public UserLogic(Repo repo)
        {
            _repo = repo;
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            return await _repo.GetAllUsers();
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _repo.GetUserById(id);
        }
    }
}
