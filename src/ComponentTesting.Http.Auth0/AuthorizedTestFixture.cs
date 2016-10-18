using System.Net.Http;
using ComponentBoundaries.Http;
using ComponentBoundaries.Http.Auth0.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentTesting.Http.Auth0
{
    public class AuthorizedTestFixture<TStartup> : TestFixture<TStartup> where TStartup : class
    {
        public AuthorizedTestFixture(Auth0Settings auth0Settings = null, TestTokenService tokenService = null, HttpMessageHandler httpMessageHandler = null)
        {
            var auth0SettingsOrDefault = auth0Settings ?? GetDefaultAuth0Settings();
            var tokenServiceOrDefault = tokenService ?? GetDefaultTokenService(auth0SettingsOrDefault);
            var httpMessageHandlerOrDefault = httpMessageHandler ?? GetDefaultHttpMessageHandler(auth0SettingsOrDefault, tokenServiceOrDefault);

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<TStartup>()
                .ConfigureServices(services =>
                {
                    services.Configure<Auth0Settings>(settings =>
                    {
                        settings.Auth0ClientId = auth0SettingsOrDefault.Auth0ClientId;
                        settings.Auth0Domain = auth0SettingsOrDefault.Auth0Domain;
                    });
                    services.AddSingleton<IHttpMessageHandlerAccessor>(provider => new HttpMessageHandlerAccessor(httpMessageHandlerOrDefault));
                });

            var server = new TestServer(webHostBuilder);
            HttpClient = server.CreateClient();
            TokenService = tokenServiceOrDefault;
        }

        public TestTokenService TokenService { get; set; }

        private static TestTokenService GetDefaultTokenService(Auth0Settings auth0SettingsOrDefault)
        {
            return  new TestTokenService(auth0SettingsOrDefault);
        }

        private static HttpMessageHandler GetDefaultHttpMessageHandler(Auth0Settings auth0Settings, TestTokenService tokenService)
        {
            var httpMessageHandler = new TestHttpMessageHandler();
            httpMessageHandler.ConfigureFakeAuth0Authority(auth0Settings, tokenService);
            return httpMessageHandler;
        }

        private static Auth0Settings GetDefaultAuth0Settings()
        {
            return new Auth0Settings
            {
                Auth0ClientId = "ID",
                Auth0Domain = "https://auth0.santa.test/"
            };
        }
    }
}