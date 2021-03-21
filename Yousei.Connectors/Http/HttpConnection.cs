using System;
using Yousei.Core;
using System.Net;
using System.Reactive.Linq;

namespace Yousei.Connectors.Http
{
    internal class HttpConnection : SimpleConnection<HttpConnection>
    {
        private readonly HttpListener listener;

        public HttpConnection(HttpListener listener)
        {
            this.listener = listener;
            HttpRequests = GetHttpRequests();

            AddTrigger("webhook", new WebhookTrigger(this));
        }

        public IObservable<WebRequest> HttpRequests { get; }

        private IObservable<WebRequest> GetHttpRequests()
            => Observable.Create<WebRequest>(async (observer, cancellationToken) =>
            {
                listener.Start();
                cancellationToken.Register(listener.Stop);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var context = await listener.GetContextAsync();
                    var dto = await WebRequest.FromRequest(context.Request);
                    observer.OnNext(dto);
                    context.Response.Close();
                }
                observer.OnCompleted();
            })
            .Publish()
            .RefCount();
    }
}