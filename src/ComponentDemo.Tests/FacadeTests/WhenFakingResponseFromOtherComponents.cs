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
using Newtonsoft.Json;
using Xunit;
using ComponentBoundaries.Http.Helpers;

namespace ComponentTesting.Demo.Tests.FacadeTests
{
    public class WhenFakingResponseFromOtherComponents
    {
        private const string ApiEndpointGetFromOtherComponent = "/api/other";
        private const string ApiEndpointPostToOtherComponent = "/api/other/data";

        [Fact(DisplayName = "OK response should be returned for get")]
        public async void AddedGetResponseShouldBeReturned()
        {
            // Arrange
            const string otherComponentBaseUrl = "https://api.othercomponent.com/";
            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AppSettings>(settings =>
            {
                settings.OtherComponentBaseUrl = otherComponentBaseUrl;
            });

            var httpMessageHandler = new TestHttpMessageHandler();
            httpMessageHandler.PushGetResponse(new Uri(new Uri(otherComponentBaseUrl), "/secret"), HttpStatusCode.OK, "Mellon!" );
            var fixture = new AuthorizedTestFixture<DemoStartup>(httpMessageHandler: httpMessageHandler, serviceCollection: serviceCollection);
            
            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointGetFromOtherComponent);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "OK response should be returned for post")]
        public async void AddedPostResponseShouldBeReturned()
        {
            // Arrange
            const string otherComponentBaseUrl = "https://api.othercomponent.com/";
            var serviceCollection = new ServiceCollection();
            serviceCollection.Configure<AppSettings>(settings =>
            {
                settings.OtherComponentBaseUrl = otherComponentBaseUrl;
            });

            var httpMessageHandler = new TestHttpMessageHandler();
            httpMessageHandler.PushResponse(
                new Tuple<HttpMethod, Uri>(
                    HttpMethod.Post,
                    new Uri(new Uri(otherComponentBaseUrl), "/data")),
                HttpStatusCode.OK,
                "Mellon!"
            );
            var fixture = new AuthorizedTestFixture<DemoStartup>(httpMessageHandler: httpMessageHandler, serviceCollection: serviceCollection);

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, ApiEndpointPostToOtherComponent);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());
            var response = await fixture.HttpClient.SendAsync(request);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsAsync<string>();
            Assert.Equal(responseString, "Mellon!");
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
            httpMessageHandler.PushGetResponse(new Uri(new Uri(otherComponentBaseUrl), "/secret"), HttpStatusCode.BadRequest, "You are saying it wrong!");
            var fixture = new AuthorizedTestFixture<DemoStartup>(httpMessageHandler: httpMessageHandler, serviceCollection: serviceCollection);

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, ApiEndpointGetFromOtherComponent);
            request.Headers.Add("Authorization", "Bearer " + fixture.TokenService.GetToken());

            // Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => fixture.HttpClient.SendAsync(request));
            Assert.Equal("Could not get secret from other!", exception.Message);
        }
    }
}
