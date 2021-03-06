using System;
using Yousei.Core;
using System.Net;
using System.Reactive.Linq;
using Yousei.Shared;

namespace Yousei.Connectors.Http
{
    internal class HttpConnection : IConnection
    {
        private readonly HttpListener? listener;

        public HttpConnection(HttpListener? listener)
        {
            this.listener = listener;
            HttpRequests = GetHttpRequests();
        }

        public IObservable<HttpRequest> HttpRequests { get; }

        public void Dispose()
        {
        }

        private IObservable<HttpRequest> GetHttpRequests()
            => Observable.Create<HttpRequest>(async (observer, cancellationToken) =>
            {
                if (listener is null)
                    throw new InvalidOperationException($"HttpListener is not available.");

                listener.Start();
                cancellationToken.Register(listener.Stop);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var context = await listener.GetContextAsync();
                    var dto = await HttpRequest.FromRequest(context.Request);
                    observer.OnNext(dto);
                    context.Response.Close();
                }
                observer.OnCompleted();
            })
            .Publish()
            .RefCount();
    }
}