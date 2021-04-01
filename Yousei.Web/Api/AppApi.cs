using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Web.Api
{
    public class AppApi : IApi
    {
        private readonly ILogger<AppApi> logger;

        private readonly GraphQlRequestHandler requestHandler;

        public AppApi(GraphQlRequestHandler requestHandler, IConfigurationDatabase database, ILogger<AppApi> logger)
        {
            ConfigurationDatabase = database;
            this.requestHandler = requestHandler;
            this.logger = logger;
        }

        public IConfigurationDatabase ConfigurationDatabase { get; }

        public async Task Reload()
        {
            var request = new GraphQLRequest
            {
                Query = @"
mutation {
    reload
}",
            };
            await requestHandler.Mutate<JToken>(request, logger);
        }
    }
}