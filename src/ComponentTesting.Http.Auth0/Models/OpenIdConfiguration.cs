using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ComponentTesting.Http.Auth0.Models
{
    public class OpenIdConfiguration
    {
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "authorization_endpoint")]
        public Uri AuthorizationEndpoint { get; set; }

        [JsonProperty(PropertyName = "token_endpoint")]
        public Uri TokenEndpoint { get; set; }

        [JsonProperty(PropertyName = "userinfo_endpoint")]
        public Uri UserinfoEndpoint { get; set; }

        [JsonProperty(PropertyName = "jwks_uri")]
        public Uri JwksUri { get; set; }
        
        [JsonProperty(PropertyName = "scopes_supported")]
        public List<string> ScopesSupported { get; set; }

        [JsonProperty(PropertyName = "response_types_supported")]
        public List<string> ResponseTypesSupported { get; set; }

        [JsonProperty(PropertyName = "response_modes_supported")]
        public List<string> ResponseModesSupported { get; set; }

        [JsonProperty(PropertyName = "subject_types_supported")]
        public List<string> SubjectTypesSupported { get; set; }

        [JsonProperty(PropertyName = "id_token_signing_alg_values_supported")]
        public List<string> IdTokenSigningAlgValuesSupported { get; set; }

        [JsonProperty(PropertyName = "token_endpoint_auth_methods_supported")]
        public List<string> TokenEndpointAuthMethodsSupported { get; set; }

        [JsonProperty(PropertyName = "claims_supported")]
        public List<string> ClaimsSupported { get; set; }
    }
}