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
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Yousei.Modules
{
    public class GraphQLModule : IModule
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

        public GraphQLModule(ILogger<GraphQLModule> logger)
        {
            this.logger = logger;
        }

        public async Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var client = new GraphQLHttpClient(args.EndPoint, new NewtonsoftJsonSerializer());

            return args.Type switch
            {
                QueryType.Query => (await SendQuery(client, args.Query, cancellationToken))
                    .Match(
                        data => Observable.Return(data),
                        () => Observable.Empty<JToken>()),
                QueryType.Subscription => Subscribe(client, args.Query, cancellationToken),
                _ => Observable.Empty<JToken>(),
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
                    logger.LogError($"{error.Message} | {JToken.FromObject(error)}");
                }

                throw new Exception("GraphQL errors.");
            }
            else
            {
                return response.Data;
            }
        }

        private IObservable<JToken> Subscribe(IGraphQLClient client, string query, CancellationToken cancellationToken)
        {
            var request = new GraphQLRequest(query);
            var observable = client.CreateSubscriptionStream<JToken>(request);
            var filteredObservable = observable.Where(o =>
            {
                if (o.Errors?.Any() ?? false)
                {
                    o.Errors.ForEach(error => logger.LogError($"{error.Message} | {JToken.FromObject(error)}"));
                    throw new Exception("GraphQL errors.");
                }
                return true;
            }).Select(o => o.Data);
            return filteredObservable;
        }
    }
}
