using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Yousei.Modules
{
    class GraphQLModule : BaseOldModule
    {
        private readonly ILogger<GraphQLModule> logger;

        private enum QueryType
        {
            Query,
            Subscription,
        }

        private class Arguments
        {
            public QueryType Type { get; set; }

            public string Query { get; set; }

            public string EndPoint { get; set; }
        }

        private class Observer : IObserver<GraphQLResponse<JToken>>, IAsyncEnumerable<JToken>
        {
            private class Enumerator : IAsyncEnumerator<JToken>
            {
                private readonly Observer observer;
                private int nextIndex = 0;
                private bool isDisposed = false;

                public Enumerator(Observer observer)
                {
                    this.observer = observer;
                }

                public JToken Current => observer.items[nextIndex - 1];

                public ValueTask DisposeAsync()
                {
                    isDisposed = true;
                    return default;
                }

                public ValueTask<bool> MoveNextAsync()
                {
                    while(!observer.isComplete && !isDisposed)
                    {
                        if (nextIndex < observer.items.Count)
                        {
                            nextIndex++;
                            return new ValueTask<bool>(true);
                        }

                        observer.autoResetEvent.WaitOne(10000);
                    }

                    return new ValueTask<bool>(false);
                }
            }

            private readonly List<JToken> items = new List<JToken>();
            private bool isComplete = false;
            private readonly ILogger logger;
            private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            public Observer(ILogger logger)
            {
                this.logger = logger;
            }

            public IAsyncEnumerator<JToken> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator(this);

            public void OnCompleted()
            {
                isComplete = true;
                autoResetEvent.Set();
            }

            public void OnError(Exception error) => logger.LogError(error.ToString());

            public void OnNext(GraphQLResponse<JToken> value)
            {
                if(value.Errors?.Any() ?? false)
                {
                    foreach(var error in value.Errors)
                    {
                        logger.LogError(error.Message);
                    }
                }
                else
                {
                    items.Add(value.Data);
                    autoResetEvent.Set();
                }
            }
        }

        public GraphQLModule(ILogger<GraphQLModule> logger)
        {
            this.logger = logger;
        }

        public string ID => "graphql";

        public override async Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var client = new GraphQLHttpClient(args.EndPoint, new NewtonsoftJsonSerializer());

            return args.Type switch
            {
                QueryType.Query => (await SendQuery(client, args.Query, cancellationToken)).ToAsyncEnumerable(),
                QueryType.Subscription => await Subscribe(client, args.Query, cancellationToken),
                _ => Enumerable.Empty<JToken>().ToAsyncEnumerable(),
            };
        }

        private async Task<Option<JToken>> SendQuery(IGraphQLClient client, string query, CancellationToken cancellationToken)
        {
            var request = new GraphQLRequest(query);
            var response = await client.SendQueryAsync<JToken>(request, cancellationToken);
            
            if(response.Errors?.Any() ?? false)
            {
                foreach(var error in response.Errors)
                {
                    logger.LogError(error.Message);
                }

                return None;
            }
            else
            {
                return response.Data;
            }
        }

        private Task<IAsyncEnumerable<JToken>> Subscribe(IGraphQLClient client, string query, CancellationToken cancellationToken)
        {
            var request = new GraphQLRequest(query);
            var observable = client.CreateSubscriptionStream<JToken>(request);
            var observer = new Observer(logger);
            observable.Subscribe(observer, cancellationToken);
            return Task.FromResult<IAsyncEnumerable<JToken>>(observer);
        }
    }
}
