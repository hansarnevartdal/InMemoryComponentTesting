using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComponentTesting.Http
{
    public class TestHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> _responseMessages = new Dictionary<Uri, HttpResponseMessage>();

        public TestHttpMessageHandler()
        {
            ThrowOn404 = true;
        }

        public bool ThrowOn404 { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public void AddResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            _responseMessages.Add(uri, responseMessage);
        }

        public void AddResponse(Uri uri, HttpStatusCode statusCode, object content)
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(
                    JsonConvert.SerializeObject(content, JsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json")
            };

            _responseMessages.Add(uri, responseMessage);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_responseMessages.ContainsKey(request.RequestUri))
            {
                var fakeResponse = await Task.FromResult(_responseMessages[request.RequestUri]);
                fakeResponse.RequestMessage = request;
                return fakeResponse;
            }
            if (ThrowOn404)
            {
                throw new Exception($"No response added for {request.RequestUri}");
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
        }
    }
}