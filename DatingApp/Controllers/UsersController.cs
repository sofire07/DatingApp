using Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DataTransfer;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserLogic _logic;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(IUserLogic logic, UserManager<ApplicationUser> userManager)
        {
            _logic = logic;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            List<UserDto> userList = await _logic.GetAllUsers();

            //Seeds DB if no users exist
            if (userList.Count == 0) {
                await _logic.SeedDatabase();
                return Ok(await _logic.GetAllUsers());
            }

            return Ok(userList);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _logic.GetUserById(id);
            if (user == null) return NotFound("User with that ID was not found.");
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _logic.GetUserByUsername(username);
            if (user == null) return NotFound("User with that username was not found.");
            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserLoggedInDto updateUser)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Name);
            var loggedIn = await _logic.GetUserByUsername(claim.Value);
            var user = await _logic.GetUserById(updateUser.Id);
            
            if (user.Id != loggedIn.Id) return Unauthorized("Unauthorized to edit this user");
            if (await _logic.EditUser(updateUser)) return NoContent();
            return BadRequest("Failed to update user");
        }
    }
}
