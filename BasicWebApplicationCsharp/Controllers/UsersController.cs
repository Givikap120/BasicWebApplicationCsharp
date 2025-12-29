using BasicWebApplicationCsharp.Domains;
using BasicWebApplicationCsharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BasicWebApplicationCsharp.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

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

            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            // Role hierarchy
            if (user.Role >= UserRole.Customer)
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));

            if (user.Role >= UserRole.Manager)
                claims.Add(new Claim(ClaimTypes.Role, "Manager"));

            if (user.Role >= UserRole.Admin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}")]
        public IActionResult ChangeRole(int id, [FromBody] UserRole role)
        {
            var user = _userService.SetRole(id, role);
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
