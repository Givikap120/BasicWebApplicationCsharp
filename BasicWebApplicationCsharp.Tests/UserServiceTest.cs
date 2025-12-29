using BasicWebApplicationCsharp.Domains;
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
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(options);
            _userService = new UserService(_context, new MyPasswordHasher());
        }

        [Fact]
        public void GetById_Should_Return_User_When_Exists()
        {
            var created = _userService.CreateUser("Alice", "alice@email.com", "password");

            var fetched = _userService.GetById(created.Id);

            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal("Alice", fetched.Username);
        }

        [Fact]
        public void GetById_Should_Return_Null_When_Not_Exists()
        {
            var user = _userService.GetById(999);

            Assert.Null(user);
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
        public void CreateUser_Should_Throw_When_Username_Exists()
        {
            _userService.CreateUser("Alice", "alice@email.com", "password");

            var ex = Assert.Throws<Exception>(() =>
                _userService.CreateUser("Alice", "alice2@email.com", "password2")
            );

            Assert.Equal("Username already exists", ex.Message);
        }

        [Fact]
        public void CreateUser_Should_Throw_When_Email_Exists()
        {
            _userService.CreateUser("Alice", "alice@email.com", "password");

            var ex = Assert.Throws<Exception>(() =>
                _userService.CreateUser("Alicia", "alice@email.com", "password2")
            );

            Assert.Equal("Email already exists", ex.Message);
        }

        [Fact]
        public void CreateUser_Should_Hash_Password()
        {
            var user = _userService.CreateUser("Alice", "alice@email.com", "password");

            var entity = _context.Users.Single();

            Assert.NotEqual("password", entity.PasswordHash);
            Assert.False(string.IsNullOrWhiteSpace(entity.PasswordHash));
        }

        [Fact]
        public void Authenticate_Should_Return_User_When_Credentials_Are_Valid()
        {
            _userService.CreateUser("Alice", "alice@email.com", "password");

            var user = _userService.Authenticate("alice@email.com", "password");

            Assert.NotNull(user);
            Assert.Equal("Alice", user.Username);
        }

        [Fact]
        public void Authenticate_Should_Return_Null_When_Email_Not_Found()
        {
            var user = _userService.Authenticate("ghost@email.com", "password");

            Assert.Null(user);
        }

        [Fact]
        public void Authenticate_Should_Return_Null_When_Password_Is_Wrong()
        {
            _userService.CreateUser("Alice", "alice@email.com", "password");

            var user = _userService.Authenticate("alice@email.com", "wrongpassword");

            Assert.Null(user);
        }

        [Fact]
        public void SetRole_Should_Update_User_Role()
        {
            var user = _userService.CreateUser("Alice", "alice@email.com", "password");

            var updated = _userService.SetRole(user.Id, UserRole.Admin);

            Assert.NotNull(updated);
            Assert.Equal(UserRole.Admin, updated.Role);

            var entity = _context.Users.Single();
            Assert.Equal((int)UserRole.Admin, entity.Role);
        }

        [Fact]
        public void SetRole_Should_Return_Null_When_User_Not_Found()
        {
            var result = _userService.SetRole(123, UserRole.Admin);

            Assert.Null(result);
        }

        [Fact]
        public void EntityFromDomain_Should_Map_All_Fields()
        {
            var domain = new User(1, "Alice", "alice@email.com", "hash", UserRole.Admin);

            var entity = _userService.EntityFromDomain(domain);

            Assert.Equal(1, entity.Id);
            Assert.Equal("Alice", entity.Username);
            Assert.Equal("alice@email.com", entity.Email);
            Assert.Equal("hash", entity.PasswordHash);
            Assert.Equal((int)UserRole.Admin, entity.Role);
        }

        [Fact]
        public void DomainFromEntity_Should_Map_All_Fields()
        {
            var entity = new UserEntity
            {
                Id = 1,
                Username = "Alice",
                Email = "alice@email.com",
                PasswordHash = "hash",
                Role = (int)UserRole.Admin
            };

            var domain = _userService.DomainFromEntity(entity);

            Assert.Equal(1, domain.Id);
            Assert.Equal("Alice", domain.Username);
            Assert.Equal("alice@email.com", domain.Email);
            Assert.Equal("hash", domain.PasswordHash);
            Assert.Equal(UserRole.Admin, domain.Role);
        }

    }

}
