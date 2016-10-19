using System.Net.Http;
using ComponentBoundaries.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ComponentBoundaries.Testing.Http
{
    public class TestFixture<TStartup> where TStartup : class
    {
        public TestFixture(HttpMessageHandler httpMessageHandler = null, IServiceCollection serviceCollection = null)
        {
            var httpMessageHandlerOrDefault = httpMessageHandler ?? new TestHttpMessageHandler();

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<TStartup>()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IHttpMessageHandlerAccessor>(provider => new HttpMessageHandlerAccessor(httpMessageHandlerOrDefault));

                    if (serviceCollection != null)
                    {
                        services.Add(serviceCollection);
                    }
                });

            var server = new TestServer(webHostBuilder);
            HttpClient = server.CreateClient();
        }

        public HttpClient HttpClient { get; set; }
    }
}