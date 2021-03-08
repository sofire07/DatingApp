using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.DataTransfer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        private readonly Mapper mapper;
        private IConfiguration configuration { get; }

        public AccountController(UserManager<ApplicationUser> _userManager, IConfiguration _configuration, Mapper _mapper)
        {
            userManager = _userManager;
            mapper = _mapper;
            configuration = _configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
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
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: configuration["JWTSettings:ValidIssuer"],
                    audience: configuration["JWTSettings:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                UserLoggedInDto ulid = mapper.ConvertUserToUserLoggedInDto(user, token);

                return Ok(ulid);
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var user = await userManager.FindByNameAsync(registerModel.UserName);
            if (user != null) return Conflict("User with that username already exists.");

            ApplicationUser newUser = new ApplicationUser
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
                UserName = registerModel.UserName,

            };

            var result = await userManager.CreateAsync(newUser, registerModel.Password);
            if (!result.Succeeded) return StatusCode(500, "User creation failed! Please check user details and try again.");

            return Ok("User created successfully!");

        }


    }
}
