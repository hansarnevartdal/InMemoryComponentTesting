using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ComponentBoundaries.Http.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComponentBoundaries.Testing.Http
{
    public class TestHttpMessageHandler : DelegatingHandler
    {
        private readonly Dictionary<Tuple<HttpMethod, Uri>, Stack<RequestResult>> _responseMessages = new Dictionary<Tuple<HttpMethod, Uri>, Stack<RequestResult>>();

        public TestHttpMessageHandler()
        {
            ThrowOn404 = true;
        }

        public bool ThrowOn404 { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public void PushGetResponse(Uri uri, HttpStatusCode statusCode, object content, bool isPermanent = false)
        {
            var requestKey = new Tuple<HttpMethod, Uri>(HttpMethod.Get, uri);

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(
                    JsonConvert.SerializeObject(content, JsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json")
            };

            PushResponse(requestKey, responseMessage, isPermanent);
        }

        public void PushResponse(Tuple<HttpMethod, Uri> requestKey, HttpStatusCode statusCode, object content, bool isPermanent = false)
        {
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(
                    JsonConvert.SerializeObject(content, JsonSerializerSettings),
                    Encoding.UTF8,
                    "application/json")
            };

            PushResponse(requestKey, responseMessage, isPermanent);
        }

        public void PushGetResponse(Uri uri, HttpResponseMessage responseMessage, bool isPermanent = false)
        {
            var requestKey = new Tuple<HttpMethod, Uri>(HttpMethod.Get, uri);
            PushResponse(requestKey, responseMessage, isPermanent);
        }

        public void PushResponse(Tuple<HttpMethod, Uri> requestKey, HttpResponseMessage responseMessage, bool isPermanent = false)
        {
            var requestResult = new RequestResult
            {
                HttpResponseMessage = responseMessage,
                IsPermanent = isPermanent
            };

            PushRequestResult(requestKey, requestResult);
        }

        public void PushHttpRequestException(Tuple<HttpMethod, Uri> requestKey, HttpRequestException httpRequestException, bool isPermanent = false)
        {
            var requestResult = new RequestResult
            {
                HttpRequestException = httpRequestException,
                IsPermanent = isPermanent
            };

            PushRequestResult(requestKey, requestResult);
        }

        private void PushRequestResult(Tuple<HttpMethod, Uri> requestKey, RequestResult requestResult)
        {
            if (!_responseMessages.ContainsKey(requestKey))
            {
                _responseMessages.Add(requestKey, new Stack<RequestResult>());
            }

            _responseMessages[requestKey].Push(requestResult);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var key = new Tuple<HttpMethod, Uri>(request.Method, request.RequestUri);
            if (_responseMessages.ContainsKey(key))
            {
                var requestResult = _responseMessages[key].Peek();
                if (!requestResult.IsPermanent)
                {
                    requestResult = _responseMessages[key].Pop();
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
                throw new Exception($"No response added for method {request.Method} on url {request.RequestUri}");
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