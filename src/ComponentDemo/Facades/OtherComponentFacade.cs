using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Component.Demo.Settings;
using ComponentBoundaries.Http;
using Microsoft.Extensions.Options;
using ComponentBoundaries.Http.Helpers;

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

        public async Task<string> PostData(List<string> data)
        {
            var httpClient = _httpClientFactory.GetHttpClient(new Uri(_appSettings.OtherComponentBaseUrl));
            var response = await httpClient.PostAsJsonAsync("/data", data);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not post data to other!");
            }

            var receipt = await response.Content.ReadAsStringAsync();
            return receipt;
        }
    }
}
