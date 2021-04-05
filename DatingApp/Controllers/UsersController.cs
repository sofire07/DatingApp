using Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DataTransfer;
using Model.Extensions;
using Model.Helpers;
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
        private readonly IPhotoLogic _photoLogic;
        private readonly ILikeLogic _likeLogic;

        public UsersController(IUserLogic logic, UserManager<ApplicationUser> userManager, IPhotoLogic photoLogic, ILikeLogic likeLogic)
        {
            _logic = logic;
            _userManager = userManager;
            _photoLogic = photoLogic;
            _likeLogic = likeLogic;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var username = User.GetUsername();
            var userList = await _logic.GetAllUsers(userParams, username);
            Response.AddPaginationHeader(userList.CurrentPage, userList.TotalPages, userList.PageSize, userList.TotalCount);
            return Ok(userList);
        }

        [HttpGet("{id}", Name = "GetUserById")]
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
            var username = User.GetUsername();
            var loggedIn = await _logic.GetUserByUsername(username);
            var user = await _logic.GetUserById(updateUser.Id);

            if (user.Id != loggedIn.Id) return Unauthorized("Unauthorized to edit this user");
            if (await _logic.EditUser(updateUser)) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        [Authorize]
        public async Task<IActionResult> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var loggedIn = await _logic.GetUserByUsername(username);

            var result = await _photoLogic.AddPhoto(loggedIn, file);
            if (result == null) return BadRequest("Problem Adding Photo.");
            return CreatedAtRoute("GetUserById", new { id = loggedIn.Id }, result);

        }

        [HttpPut("set-main-photo/{photoId}")]
        [Authorize]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            var username = User.GetUsername();
            var loggedIn = await _logic.GetUserByUsername(username);
            if (await _photoLogic.UpdateMainPhoto(loggedIn, photoId) == false) return BadRequest("Failed to set main photo.");
            return NoContent();

        }

        [HttpDelete("delete-photo/{photoId}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUsername();
            var loggedIn = await _logic.GetUserByUsername(username);
            var result = await _photoLogic.DeletePhoto(loggedIn, photoId);
            if (result == null) return BadRequest("Failed to delete photo. Photo does not exist.");
            if (result == false) return BadRequest("Failed to delete photo.");
            return NoContent();
        }

        [HttpPost("like/{username}")]
        [Authorize]
        public async Task<IActionResult> AddLike(string username)
        {
            var loggedInUsername = User.GetUsername();
            var result = await _likeLogic.AddLike(loggedInUsername, username);
            if (result == null) return BadRequest("Failed to add like. Like already exists.");
            if (result == true) return NoContent();
            return BadRequest("There was an issue adding this like");
        }

        [HttpDelete("like/{username}")]
        [Authorize]
        public async Task<IActionResult> RemoveLike(string username)
        {
            var loggedInUsername = User.GetUsername();
            var result = await _likeLogic.RemoveLike(loggedInUsername, username);
            if (result == null) return BadRequest("Failed to remove like. Like does not exist.");
            if (result == true) return NoContent();
            return BadRequest("There was an issue removing this like");
        }

        [HttpGet("liked-by")]
        [Authorize]
        public async Task<IActionResult> GetLikedBy([FromQuery] LikesParams likeParams)
        {
            var loggedInUsername = User.GetUsername();
            return Ok(await _likeLogic.GetLikedBy(loggedInUsername));
        }

        [HttpGet("liking")]
        [Authorize]
        public async Task<IActionResult> GetLiking()
        {
            var loggedInUsername = User.GetUsername();
            return Ok(await _likeLogic.GetLiking(loggedInUsername));
        }

    }
}
