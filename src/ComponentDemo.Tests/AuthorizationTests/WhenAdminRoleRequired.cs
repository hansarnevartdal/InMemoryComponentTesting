using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Component.Demo;
using ComponentBoundaries.Testing.Http.Auth0;
using Xunit;

namespace ComponentTesting.Demo.Tests.AuthorizationTests
{
    public class WhenAdminRoleRequired
    {
        private const string ApiEndpointRquiringAdminRole = "/api/require/role/admin";

        [Fact(DisplayName = "Token with admin role is given access")]
        public async void TokenWithAdminRoleIsAuthorized()
        {
            // Arrange
            var fixture = new AuthorizedTestFixture<DemoStartup>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Administrator")
            };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointRquiringAdminRole);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken(claims));
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Token without admin role is forbidden access")]
        public async void TokenWithoutAdminRoleIsForbiddenAccess()
        {
            // Arrange
            var fixture = new AuthorizedTestFixture<DemoStartup>();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointRquiringAdminRole);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}