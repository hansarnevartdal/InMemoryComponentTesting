using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using ComponentBoundaries.Http.Auth0.Settings;
using ComponentBoundaries.Testing.Http.Auth0.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace ComponentBoundaries.Testing.Http.Auth0
{
    public class TestTokenService
    {
        private readonly Auth0Settings _appSettings;
        private readonly X509Certificate2 _signatureValidationCertificate;
        private readonly SigningCredentials _signingCredentials;

        public TestTokenService(Auth0Settings appSettings)
        {
            _appSettings = appSettings;

            var signingInformation = SignatureHelper.GenerateSigningInformation();
            _signingCredentials = signingInformation.Item1;
            _signatureValidationCertificate = signingInformation.Item2;
        }

        public string GetToken(List<Claim> claims = null)
        {
            var identity = GetIdentity(claims);
            var expires = DateTime.Now.AddHours(1);
            var notBefore = DateTime.Now.AddHours(-1);

            return GetToken(identity, expires, notBefore);
        }

        public string GetExpiredToken(List<Claim> claims = null)
        {
            var identity = GetIdentity(claims);
            var expires = DateTime.Now.AddHours(-1);
            var notBefore = DateTime.Now.AddHours(-2);

            return GetToken(identity, expires, notBefore);
        }

        private string GetToken(ClaimsIdentity claimsIdentity, DateTime expires, DateTime notBefore)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _appSettings.Auth0Domain,
                Audience = _appSettings.Auth0ClientId,
                Subject = claimsIdentity,
                Expires = expires,
                NotBefore = notBefore,
                SigningCredentials = _signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(plainToken);
        }

        private ClaimsIdentity GetIdentity(List<Claim> claims = null)
        {
            return new ClaimsIdentity(claims ?? GetDefaultClaims(), "TestTokenService");
        }

        private List<Claim> GetDefaultClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "Test McCase")
            };
        }

        public string GetSigningKey()
        {
            return Convert.ToBase64String(_signatureValidationCertificate.RawData);
        }
    }
}