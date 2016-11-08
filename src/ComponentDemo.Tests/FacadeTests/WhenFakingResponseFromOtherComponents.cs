using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Component.Demo;
using Component.Demo.Settings;
using ComponentBoundaries.Testing.Http;
using ComponentBoundaries.Testing.Http.Auth0;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ComponentTesting.Demo.Tests.FacadeTests
{
    public class WhenFakingResponseFromOtherComponents
    {
        private const string ApiEndpointCallingOtherComponent = "/api/other";

        [Fact(DisplayName = "OK response should be returned")]
        public async void AddedResponseShouldBeReturned()
        {
            // Arrange
            const string otherComponentBaseUrl = "https://api.othercomponent.com/";
            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AppSettings>(settings =>
            {
                settings.OtherComponentBaseUrl = otherComponentBaseUrl;
            });

            var httpMessageHandler = new TestHttpMessageHandler();
            httpMessageHandler.PushResponse(new Uri(new Uri(otherComponentBaseUrl), "/secret"), HttpStatusCode.OK, "Mellon!" );
            var fixture = new AuthorizedTestFixture<DemoStartup>(httpMessageHandler: httpMessageHandler, serviceCollection: serviceCollection);
            
            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointCallingOtherComponent);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Error response should throw")]
        public async void ErrorResponseShouldThrow()
        {
            // Arrange
            const string otherComponentBaseUrl = "https://api.othercomponent.com/";
            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AppSettings>(settings =>
            {
                settings.OtherComponentBaseUrl = otherComponentBaseUrl;
            });

            var httpMessageHandler = new TestHttpMessageHandler();
            httpMessageHandler.PushResponse(new Uri(new Uri(otherComponentBaseUrl), "/secret"), HttpStatusCode.BadRequest, "You are saying it wrong!");
            var fixture = new AuthorizedTestFixture<DemoStartup>(httpMessageHandler: httpMessageHandler, serviceCollection: serviceCollection);

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointCallingOtherComponent);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());

            // Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => fixture.HttpClient.SendAsync(request));
            Assert.Equal("Could not get secret from other!", exception.Message);
        }
    }
}
