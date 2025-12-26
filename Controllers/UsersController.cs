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
        public IActionResult Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                var user = _userService.CreateUser(dto.Username, dto.Email, dto.Password);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            var user = _userService.Authenticate(dto.Email, dto.Password);
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

        public class UserRegisterDto
        {
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public class UserLoginDto
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}
