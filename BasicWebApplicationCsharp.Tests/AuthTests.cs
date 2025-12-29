using BasicWebApplicationCsharp.Domains;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using static BasicWebApplicationCsharp.Tests.TestAuthHandler;

namespace BasicWebApplicationCsharp.Tests
{
    public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TestAuthOptions _testOptions;

        public AuthTests(WebApplicationFactory<Program> factory)
        {
            _testOptions = new TestAuthOptions();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = "jwt_key_made_specifically_for_testing_this_app"
                    });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(_testOptions);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            }).CreateClient();
        }
        private void Authorize(UserRole role)
        {
            _testOptions.IsAuthenticated = true;
            _testOptions.Role = role;
        }

        [Fact]
        public async Task Login_Should_Return_Jwt_When_Credentials_Are_Valid()
        {
            await _client.PostAsJsonAsync("/users/register", new
            {
                username = "alice",
                email = "alice@test.com",
                password = "password"
            });

            var response = await _client.PostAsJsonAsync("/users/login", new
            {
                email = "alice@test.com",
                password = "password"
            });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.GetProperty("token").GetString();

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task Login_Should_Return_401_When_Password_Is_Wrong()
        {
            var response = await _client.PostAsJsonAsync("/users/login", new
            {
                email = "nope@test.com",
                password = "wrong"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_Should_Reject_Unauthorized()
        {
            var response = await _client.GetAsync("/orders/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_Should_Reject_Customer()
        {
            Authorize(UserRole.Customer);

            var response = await _client.GetAsync("/orders/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_Should_Allow_Manager()
        {
            Authorize(UserRole.Manager);

            var response = await _client.GetAsync("/orders/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ManagerEndpoint_Should_Allow_Admin()
        {
            Authorize(UserRole.Admin);

            var response = await _client.GetAsync("/orders/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task AdminEndpoint_Should_Reject_Unauthorized()
        {
            var response = await _client.GetAsync("/users/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_Should_Reject_Customer()
        {
            Authorize(UserRole.Customer);

            var response = await _client.GetAsync("/users/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_Should_Reject_Manager()
        {
            Authorize(UserRole.Manager);

            var response = await _client.GetAsync("/users/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AdminEndpoint_Should_Allow_Admin()
        {
            Authorize(UserRole.Admin);

            var response = await _client.GetAsync("/users/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


    }

}
