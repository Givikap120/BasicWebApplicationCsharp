using BasicWebApplicationCsharp.Entities;
using BasicWebApplicationCsharp.Services;
using BasicWebApplicationCsharp.Utils;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApplicationCsharp.Tests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly AppDbContext _context;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options;
            _context = new AppDbContext(options);

            var passwordHasher = new MyPasswordHasher();
            _userService = new UserService(_context, passwordHasher);
        }

        [Fact]
        public void CreateUser_Should_Add_User_To_Db()
        {
            var user = _userService.CreateUser("Alice", "alice@email.com", "password");

            Assert.NotNull(user);
            Assert.Equal("Alice", user.Username);
            Assert.Single(_context.Users); // Only one user exists
        }

        [Fact]
        public void CreateUser_Should_Throw_When_Email_Exists()
        {
            _userService.CreateUser("Bob", "bob@email.com", "password");

            var ex = Assert.Throws<Exception>(() =>
                _userService.CreateUser("Bobby", "bob@email.com", "password2")
            );

            Assert.Equal("Email already exists", ex.Message);
        }
    }

}
