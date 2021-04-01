using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Web.Api
{
    public class GraphQlRequestHandler
    {
        private readonly GraphQLHttpClient client;

        private readonly ILogger<GraphQlRequestHandler> logger;

        public GraphQlRequestHandler(IOptions<ApiOptions> options, ILogger<GraphQlRequestHandler> logger)
        {
            client = new GraphQLHttpClient(options.Value.Url, new NewtonsoftJsonSerializer());
            this.logger = logger;
        }

        public Task<T> Mutate<T>(GraphQLRequest request, ILogger logger = default)
            => Request(request, client.SendMutationAsync<T>, logger);

        public Task<T> Query<T>(GraphQLRequest request, ILogger logger = default)
                    => Request(request, client.SendQueryAsync<T>, logger);

        private async Task<T> Request<T>(GraphQLRequest request, Func<GraphQLRequest, CancellationToken, Task<GraphQLResponse<T>>> sendFunc, ILogger logger)
        {
            logger ??= this.logger;
            logger.LogTrace($"<< Query: {request.Query}; Variables: {request.Variables}");
            var response = await sendFunc(request, default);
            logger.LogTrace($">> Data: {response.Data}");

            foreach (var error in response.Errors ?? Enumerable.Empty<GraphQLError>())
            {
                var path = error.Path is null
                    ? string.Empty
                    : string.Join(".", error.Path);
                var extensions = error.Extensions is null
                    ? string.Empty
                    : string.Concat(error.Extensions.Select(o => $"\n{o.Key,-20}: {o.Value}"));
                logger.LogWarning($"{error.Message} @ {path}{extensions}");
            }

            if (response.Extensions is not null)
            {
                logger.LogTrace(string.Join("\n", response.Extensions.Select(o => $"{o.Key,-20}: {o.Value}")));
            }

            return response.Data;
        }
    }
}