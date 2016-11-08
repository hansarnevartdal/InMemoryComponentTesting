using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComponentBoundaries.Http.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            return await httpClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }
}