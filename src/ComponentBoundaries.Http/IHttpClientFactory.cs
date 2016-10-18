using System;
using System.Net.Http;

namespace ComponentBoundaries.Http
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient(Uri baseUri);
    }
}