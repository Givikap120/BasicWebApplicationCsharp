using BasicWebApplicationCsharp.Domains;
using Microsoft.AspNetCore.Identity;

namespace BasicWebApplicationCsharp.Utils
{
    public class MyPasswordHasher : PasswordHasher<User>, IPasswordHasher
    {
        // We will pass user as null since we don't need it for our hashing
#pragma warning disable CS8625
        public string HashPassword(string password) => HashPassword(null, password);
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword) => VerifyHashedPassword(null, hashedPassword, providedPassword);
#pragma warning restore CS8625
    }
}
