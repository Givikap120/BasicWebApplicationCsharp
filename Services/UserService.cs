using BasicWebApplicationCsharp.Domains;
using BasicWebApplicationCsharp.Entities;
using BasicWebApplicationCsharp.Utils;
using Microsoft.AspNetCore.Identity;

namespace BasicWebApplicationCsharp.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(AppDbContext db, IPasswordHasher passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public UserEntity EntityFromDomain(User user)
        {
            return new UserEntity
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = (int)user.Role
            };
        }

        public User DomainFromEntity(UserEntity user)
        {
            return new User(user.Id, user.Username, user.Email, user.PasswordHash, (UserRole)user.Role);
        }

        public User? GetById(int id)
        {
            var userEntity = _db.Users.Find(id);
            return userEntity == null ? null : DomainFromEntity(userEntity);
        }

        public User CreateUser(string username, string email, string password)
        {
            if (_db.Users.Any(u => u.Username == username))
                throw new Exception("Username already exists");

            if (_db.Users.Any(u => u.Email == email))
                throw new Exception("Email already exists");

            var user = new UserEntity
            {
                Username = username,
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(password),
                Role = (int)UserRole.Customer
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return DomainFromEntity(user);
        }

        public User? Authenticate(string email, string password)
        {
            var user = _db.Users.SingleOrDefault(u => u.Email == email);

            if (user == null || _passwordHasher.VerifyHashedPassword(password, user.PasswordHash) == PasswordVerificationResult.Failed)
               return null;

            return DomainFromEntity(user);
        }
    }
}
