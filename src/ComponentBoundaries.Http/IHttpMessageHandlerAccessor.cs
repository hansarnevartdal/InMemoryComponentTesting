using System.Net.Http;

namespace ComponentBoundaries.Http
{
    public interface IHttpMessageHandlerAccessor
    {
        HttpMessageHandler Value { get; }
        bool HandlerDefined { get; }
    }
}