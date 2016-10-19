using System.Net;
using System.Net.Http;
using Component.Demo;
using ComponentBoundaries.Testing.Http.Auth0;
using Xunit;

namespace ComponentTesting.Demo.Tests.AuthorizationTests
{
    public class WhenAuthorizationRequired
    {
        private const string ApiEndpointRequiringAuthorization = "/api/require/authorization";

        [Fact(DisplayName = "Valid token is authorized")]
        public async void ValidTokenIsAuthorized()
        {
            // Arrange
            var fixture = new AuthorizedTestFixture<DemoStartup>();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointRequiringAuthorization);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Expired token is unauthorized")]
        public async void ExpiredTokenIsUnauthorized()
        {
            // Arrange
            var fixture = new AuthorizedTestFixture<DemoStartup>();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointRequiringAuthorization);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetExpiredToken());
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
