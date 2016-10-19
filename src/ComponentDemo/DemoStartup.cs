using Component.Demo.Facades;
using Component.Demo.Settings;
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
                .Configure<Auth0Settings>(Configuration.GetSection("Auth0Settings"))
                .Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.TryAddSingleton<IHttpMessageHandlerAccessor, HttpMessageHandlerAccessor>();
            services.TryAddSingleton<IHttpClientFactory, HttpClientFactory>();
            services.AddScoped<IOtherComponentFacade, OtherComponentFacade>();

            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IOptions<Auth0Settings> auth0SettingsAccessor, 
            IHttpMessageHandlerAccessor httpMessageHandlerAccessor)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
            
            app.UseAuth0Tokens(auth0SettingsAccessor, httpMessageHandlerAccessor, loggerFactory);
            app.UseMvc();
        }
    }
}
