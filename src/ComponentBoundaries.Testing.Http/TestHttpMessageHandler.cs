using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComponentBoundaries.Testing.Http
{
    public class TestHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, Stack<RequestResult>> _responseMessages = new Dictionary<Uri, Stack<RequestResult>>();

        public TestHttpMessageHandler()
        {
            ThrowOn404 = true;
        }

        public bool ThrowOn404 { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public void PushResponse(Uri uri, HttpStatusCode statusCode, object content, bool isPermanent = false)
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(
                    JsonConvert.SerializeObject(content, JsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json")
            };

            PushResponse(uri, responseMessage, isPermanent);
        }

        public void PushResponse(Uri uri, HttpResponseMessage responseMessage, bool isPermanent = false)
        {
            var requestResult = new RequestResult
            {
                HttpResponseMessage = responseMessage,
                IsPermanent = isPermanent
            };

            PushRequestResult(uri, requestResult);
        }

        public void PushHttpRequestException(Uri uri, HttpRequestException httpRequestException, bool isPermanent = false)
        {
            var requestResult = new RequestResult
            {
                HttpRequestException = httpRequestException,
                IsPermanent = isPermanent
            };

            PushRequestResult(uri, requestResult);
        }

        private void PushRequestResult(Uri uri, RequestResult requestResult)
        {
            if (!_responseMessages.ContainsKey(uri))
            {
                _responseMessages.Add(uri, new Stack<RequestResult>());
            }

            _responseMessages[uri].Push(requestResult);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_responseMessages.ContainsKey(request.RequestUri))
            {
                var requestResult = _responseMessages[request.RequestUri].Peek();
                if (!requestResult.IsPermanent)
                {
                    requestResult = _responseMessages[request.RequestUri].Pop();
                }

                if (requestResult.ThrowsException)
                {
                    throw requestResult.HttpRequestException;
                }

                var fakeResponse = await Task.FromResult(requestResult.HttpResponseMessage);
                fakeResponse.RequestMessage = request;
                return fakeResponse;
            }

            if (ThrowOn404)
            {
                throw new Exception($"No response added for {request.RequestUri}");
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
        }

        private class RequestResult
        {
            public HttpResponseMessage HttpResponseMessage { get; set; }
            public HttpRequestException HttpRequestException { get; set; }
            public bool ThrowsException => HttpRequestException != null;
            public bool IsPermanent { get; set; }
        }
    }
}