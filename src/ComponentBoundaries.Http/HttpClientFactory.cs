using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace ComponentBoundaries.Http
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _httpMessageHandler;
        private readonly ConcurrentDictionary<Uri, HttpClient> _httpClients;

        public HttpClientFactory(IHttpMessageHandlerAccessor httpMessageHandlerAccessor)
        {
            _httpMessageHandler = httpMessageHandlerAccessor.Value;
            if (_httpClients == null)
            {
                _httpClients = new ConcurrentDictionary<Uri, HttpClient>();
            }
        }

        public HttpClient GetHttpClient(Uri baseAddress)
        {
            return _httpClients.GetOrAdd(baseAddress, type =>
            {
                var httpClient = _httpMessageHandler == null ? new HttpClient() : new HttpClient(_httpMessageHandler);
                httpClient.BaseAddress = baseAddress;
                return httpClient;
            });
        }
    }
}