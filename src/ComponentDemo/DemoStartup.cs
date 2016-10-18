using ComponentBoundaries.Http;
using ComponentBoundaries.Http.Auth0;
using ComponentBoundaries.Http.Auth0.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Component.Demo
{
    public class DemoStartup
    {
        public DemoStartup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions()
                .Configure<Auth0Settings>(Configuration.GetSection("Auth0Settings"));

            services.TryAddSingleton<IHttpMessageHandlerAccessor, HttpMessageHandlerAccessor>();
            services.TryAddSingleton<IHttpClientFactory, HttpClientFactory>();

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IOptions<Auth0Settings> appSettingsAccessor, 
            IHttpMessageHandlerAccessor httpMessageHandlerAccessor)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
            
            app.UseAuth0Tokens(appSettingsAccessor, httpMessageHandlerAccessor, loggerFactory);
            app.UseMvc();
        }
    }
}
