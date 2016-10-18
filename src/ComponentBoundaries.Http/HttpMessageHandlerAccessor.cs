using System.Net.Http;

namespace ComponentBoundaries.Http
{
    public class HttpMessageHandlerAccessor : IHttpMessageHandlerAccessor
    {
        public HttpMessageHandlerAccessor()
        {
            Value = null;
        }

        public HttpMessageHandlerAccessor(HttpMessageHandler value)
        {
            Value = value;
        }

        public HttpMessageHandler Value { get; }

        public bool HandlerDefined => Value != null;
    }
}