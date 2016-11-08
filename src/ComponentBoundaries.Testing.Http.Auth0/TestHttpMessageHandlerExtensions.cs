using System;
using System.Collections.Generic;
using System.Net;
using ComponentBoundaries.Http.Auth0.Settings;
using ComponentBoundaries.Testing.Http.Auth0.Models;

namespace ComponentBoundaries.Testing.Http.Auth0
{
    public static class TestHttpMessageHandlerExtensions
    {
        private const string WellKnownJwks = "/.well-known/jwks.json";
        private const string WellKnownOpenIdConfiguartion = "/.well-known/openid-configuration";

        public static void ConfigureFakeAuth0Authority(
            this TestHttpMessageHandler testHttpMessageHandler, 
            Auth0Settings appSettings,
            TestTokenService tokenService)
        {
            var jwksUri = new Uri(new Uri(appSettings.Auth0Domain), WellKnownJwks);
            testHttpMessageHandler.PushResponse(jwksUri,
                HttpStatusCode.OK,
                new Jwks
                {
                    Keys = new List<Key>
                    {
                        new Key
                        {
                            Alg = "RS256",
                            Kty = "RSA",
                            Use = "sig",
                            X5C = new List<string>
                            {
                                tokenService.GetSigningKey()
                            }
                        }
                    }
                },
                true
            );

            testHttpMessageHandler.PushResponse(new Uri(new Uri(appSettings.Auth0Domain), WellKnownOpenIdConfiguartion),
                HttpStatusCode.OK,
                new OpenIdConfiguration
                {
                    Issuer = appSettings.Auth0Domain,
                    JwksUri = jwksUri,
                    IdTokenSigningAlgValuesSupported = new List<string>
                    {
                        "RS256"
                    }
                },
                true
            );
        }
    }
}