using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {

        private readonly UserRepo _repo;
        public BuggyController(UserRepo repo)
        {
            _repo = repo;
        }

        [Authorize]
        [HttpGet("auth")]
        public IActionResult GetSecret()
        {
            return Ok("secret text");
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("server-error")]
        public async Task<ActionResult<string>> GetServerError()
        {
                var user = await _repo.GetUserById("id");
                var thingToReturn = user.ToString();
                return thingToReturn;            
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("This is a bad request.");
        }
    }
}
