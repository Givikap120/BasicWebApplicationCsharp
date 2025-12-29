using Microsoft.AspNetCore.Identity;

namespace BasicWebApplicationCsharp.Utils
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
