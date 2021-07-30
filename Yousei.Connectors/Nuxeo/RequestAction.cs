using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Nuxeo
{
    internal record RequestArguments
    {
        public IParameter<string> Method { get; init; } = "GET".ToConstantParameter();

        public IParameter<string> Endpoint { get; init; } = DefaultParameter<string>.Instance;

        public IParameter<string> ContentType { get; init; } = "application/json".ToConstantParameter();

        public IParameter<Dictionary<string, string>> Headers { get; init; } = DefaultParameter<Dictionary<string, string>>.Instance;

        public IParameter<JToken> Data { get; init; } = DefaultParameter<JToken>.Instance;

        public IParameter<Dictionary<string, string>> Parameters { get; init; } = DefaultParameter<Dictionary<string, string>>.Instance;
    }

    internal class RequestAction : NuxeoAction<RequestArguments>
    {
        public override string Name { get; } = "request";

        protected override async Task Act(IFlowContext context, RequestArguments? arguments, Client client)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var method = await arguments.Method.Resolve(context);
            var requestType = method switch
            {
                "GET" => Client.RequestType.GET,
                "POST" => Client.RequestType.POST,
                "PUT" => Client.RequestType.PUT,
                "DELETE" => Client.RequestType.DELETE,
                _ => throw new NotSupportedException(),
            };
            var endpoint = await arguments.Endpoint.Resolve(context);
            var parametersDict = await arguments.Parameters.Resolve(context);
            var data = await arguments.Data.Resolve(context);
            var additionalHeaders = await arguments.Headers.Resolve(context);
            var contentType = await arguments.ContentType.Resolve(context);

            var parameters = new QueryParams();
            if (parametersDict is not null)
                foreach (var (key, value) in parametersDict)
                    parameters.Add(key, value);

            var result = await client.Request(requestType, endpoint, parameters, data, additionalHeaders, contentType);
            await context.SetData(result);
        }
    }
}