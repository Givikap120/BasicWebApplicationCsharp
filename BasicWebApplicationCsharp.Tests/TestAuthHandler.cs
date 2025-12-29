using BasicWebApplicationCsharp.Domains;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BasicWebApplicationCsharp.Tests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly TestAuthOptions _testOptions;

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, TestAuthOptions testOptions) 
            : base(options, logger, encoder)
        {
            _testOptions = testOptions;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_testOptions.IsAuthenticated)
                return Task.FromResult(AuthenticateResult.Fail("Not authenticated"));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "TestUser")
            };

            if (_testOptions.Role >= UserRole.Customer)
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));
            if (_testOptions.Role >= UserRole.Manager)
                claims.Add(new Claim(ClaimTypes.Role, "Manager"));
            if (_testOptions.Role >= UserRole.Admin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public class TestAuthOptions
        {
            public bool IsAuthenticated { get; set; } = false;
            public UserRole Role { get; set; } = UserRole.Customer;
        }
    }

}
