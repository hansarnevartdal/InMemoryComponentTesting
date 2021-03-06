﻿using ComponentBoundaries.Http;
using ComponentBoundaries.Http.Auth0.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ComponentBoundaries.Testing.Http.Auth0
{
    public class AuthorizedTestFixture<TStartup> : TestFixture<TStartup> where TStartup : class
    {
        public AuthorizedTestFixture(Auth0Settings auth0Settings = null, TestTokenService tokenService = null, TestHttpMessageHandler httpMessageHandler = null, IServiceCollection serviceCollection = null)
        {
            var auth0SettingsOrDefault = auth0Settings ?? GetDefaultAuth0Settings();
            var tokenServiceOrDefault = tokenService ?? GetDefaultTokenService(auth0SettingsOrDefault);
            var httpMessageHandlerOrDefault = httpMessageHandler ?? GetDefaultHttpMessageHandler();
            httpMessageHandlerOrDefault.ConfigureFakeAuth0Authority(auth0SettingsOrDefault, tokenServiceOrDefault);

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

                    if (serviceCollection != null)
                    {
                        services.Add(serviceCollection);
                    }
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

        private static TestHttpMessageHandler GetDefaultHttpMessageHandler()
        {
            return new TestHttpMessageHandler();
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