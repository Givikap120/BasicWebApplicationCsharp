namespace BasicWebApplicationCsharp.Domains
{
    public enum UserRole
    {
        Admin,
        Manager,
        Customer
    }

    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string PasswordHash { get; private set; } = "";
        public UserRole Role { get; private set; } = UserRole.Customer;
    }
}
