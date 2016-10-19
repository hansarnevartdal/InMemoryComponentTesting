using System;
using System.Threading.Tasks;
using Component.Demo.Settings;
using ComponentBoundaries.Http;
using Microsoft.Extensions.Options;

namespace Component.Demo.Facades
{
    public class OtherComponentFacade : IOtherComponentFacade
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;

        public OtherComponentFacade(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettingsAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _appSettings = appSettingsAccessor.Value;
        }

        public async Task<string> GetSecret()
        {
            var httpClient = _httpClientFactory.GetHttpClient(new Uri(_appSettings.OtherComponentBaseUrl));
            var response = await httpClient.GetAsync("/secret");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not get secret from other!");
            }

            var secret = await response.Content.ReadAsStringAsync();
            return secret;
        }
    }
}
