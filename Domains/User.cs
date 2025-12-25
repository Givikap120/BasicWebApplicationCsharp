namespace BasicWebApplicationCsharp.Domains
{
    public enum UserRole
    {
        Customer,
        Admin,
        Manager,
    }

    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string PasswordHash { get; private set; } = "";
        public UserRole Role { get; private set; } = UserRole.Customer;

        public User(int id, string username, string email, string passwordHash, UserRole role)
        {
            Id = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
