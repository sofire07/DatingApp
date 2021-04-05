using AutoMapper;
using Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.DataTransfer;
using Model.Extensions;
using Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IUserLogic userLogic;
        private readonly IMapper mapper;
        private readonly IUserRepo repo;
        private IConfiguration configuration { get; }


        public AccountController(UserManager<ApplicationUser> _userManager, 
            IConfiguration _configuration, 
            IMapper _mapper, 
            IUserRepo _repo, 
            IUserLogic _userLogic,
            RoleManager<ApplicationRole> _roleManager)
        {
            userManager = _userManager;
            mapper = _mapper;
            configuration = _configuration;
            repo = _repo;
            userLogic = _userLogic;
            roleManager = _roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await repo.GetUserByUsername(model.UserName);
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        authClaims.Add(new Claim("role", userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: configuration["JWTSettings:ValidIssuer"],
                        audience: configuration["JWTSettings:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                    user.LastActive = DateTime.Now;
                    UserLoggedInDto ulid = mapper.Map<UserLoggedInDto>(user);
                    await userLogic.EditUser(ulid);
                    ulid.Token = new JwtSecurityTokenHandler().WriteToken(token);
                    ulid.TokenExpires = token.ValidTo;
                    return Ok(ulid);
                }
                else {
                    return Unauthorized("Incorrect password.");
                }
            }
            return Unauthorized("Username not registered");
        }

        [HttpPost("register")]
        public async Task<ActionResult<bool>> Register([FromBody] RegisterModel registerModel)
        {
            var user = await userManager.FindByNameAsync(registerModel.UserName);
            if (user != null) return Conflict("User with that username already exists.");

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
                await roleManager.CreateAsync(new ApplicationRole { Name = "Moderator" });
                await roleManager.CreateAsync(new ApplicationRole { Name = "User" });
            }

            ApplicationUser newUser = new ApplicationUser
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
                UserName = registerModel.UserName,
                DoB = registerModel.DoB,
                City = registerModel.City,
                State = registerModel.State,
                Interests = registerModel.Interests,
                LookingFor = registerModel.LookingFor,
                Gender = registerModel.Gender.ToLower(),
            };

            var result = await userManager.CreateAsync(newUser, registerModel.Password);
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (IdentityError ie in result.Errors) {
                    sb.Append(ie.Description + " ");
                }
                return Unauthorized(sb.ToString());
            }
            //if(await userManager.GetUsersInRoleAsync("Admin") == null)
            //{
            //    userManager.AddToRoleAsync(newUser, "Admin");
            //}
            await userManager.AddToRoleAsync(newUser, "User");

            return true;

        }

        [HttpPost("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] EditPassword ep)
        {
            var username = User.GetUsername();
            var user = await userManager.FindByNameAsync(username);
            var result = await userManager.ChangePasswordAsync(user, ep.OldPassword, ep.NewPassword);
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (IdentityError ie in result.Errors)
                {
                    sb.Append(ie.Description + " ");
                }
                return BadRequest(sb.ToString());
            }
            return NoContent();
        }


    }
}
