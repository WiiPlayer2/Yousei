using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
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
        private readonly GraphQLHttpClient client;

        public AppApi(IOptions<ApiOptions> options)
        {
            client = new GraphQLHttpClient(options.Value.Url, new NewtonsoftJsonSerializer());
            ConfigurationDatabase = new AppConfigurationDatabase(client);
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
            await client.SendMutationAsync<JToken>(request);
        }
    }
}