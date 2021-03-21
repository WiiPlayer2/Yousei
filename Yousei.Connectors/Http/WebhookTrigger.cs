﻿using System;
using Yousei.Core;
using System.Reactive.Linq;
using System.Linq;

namespace Yousei.Connectors.Http
{
    internal class WebhookTrigger : FlowTrigger<WebhookArguments>
    {
        private readonly HttpConnection httpConnection;

        public WebhookTrigger(HttpConnection httpConnection)
        {
            this.httpConnection = httpConnection;
        }

        protected override IObservable<object> GetEvents(WebhookArguments arguments)
            => httpConnection.HttpRequests
                .Where(o => string.IsNullOrEmpty(arguments.Path) || o.Url.AbsolutePath.StartsWith(arguments.Path));
    }
}