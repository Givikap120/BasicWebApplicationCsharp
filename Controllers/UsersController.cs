using BasicWebApplicationCsharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebApplicationCsharp.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
            => _userService = userService;

        [HttpPost("register")]
        public IActionResult Register(string username, string email, string password)
        {
            try
            {
                var user = _userService.CreateUser(username, email, password);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = _userService.Authenticate(email, password);
            if (user == null)
                return Unauthorized();

            return Ok(user);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
