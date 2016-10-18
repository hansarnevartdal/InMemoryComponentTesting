using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComponentBoundaries.Http.Helpers
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent httpContent)
        {
            var jsonStream = await httpContent.ReadAsStreamAsync();

            var serializer = new JsonSerializer();
            T data;
            using (var streamReader = new StreamReader(jsonStream))
            {
                data = (T)serializer.Deserialize(streamReader, typeof(T));
            }
            return data;
        }
    }
}