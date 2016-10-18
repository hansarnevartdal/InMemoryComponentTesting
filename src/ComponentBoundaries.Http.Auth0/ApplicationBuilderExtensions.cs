using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ComponentBoundaries.Http.Auth0.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ComponentBoundaries.Http.Auth0
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseAuth0Tokens(
            this IApplicationBuilder app, 
            IOptions<Auth0Settings> appSettingsAccessor, 
            IHttpMessageHandlerAccessor httpMessageHandlerAccessor,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Auth0");
            var appSettings = appSettingsAccessor.Value;

            var jwtOptions = new JwtBearerOptions
            {
                Authority = appSettings.Auth0Domain,
                AutomaticAuthenticate = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = appSettings.Auth0Domain,
                    ValidAudiences = new List<string> {appSettings.Auth0ClientId},
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ClockSkew = new TimeSpan(0, 10, 0)
                },
                Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError("OnAuthenticationFailed", context.Exception);
                        return Task.FromResult(0);
                    },
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;
                        claimsIdentity?.AddClaim(new Claim("id_token",
                            context.Request.Headers["Authorization"][0].Substring(
                                context.Ticket.AuthenticationScheme.Length + 1)));

                        return Task.FromResult(0);
                    }
                }
            };

            if (httpMessageHandlerAccessor.HandlerDefined)
            {
                jwtOptions.BackchannelHttpHandler = httpMessageHandlerAccessor.Value;
            }

            app.UseJwtBearerAuthentication(jwtOptions);
        }
    }
}