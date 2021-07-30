using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Yousei.Web.Api;

namespace Yousei.Web.Model
{
    public class FlowConfigModel : ConfigModel
    {
        private static string getFlowQuery = @"
query _($flowName: String!) {
  database {
    flow(name: $flowName) {
      config {
        content
        language
      }
    }
  }
}";

        private static string setFlowQuery = @"
mutation _($flowName: String!, $source: SourceConfigInput) {
  setFlow(name: $flowName, source: $source) {
    name
  }
}";

        private readonly string flowName;

        public FlowConfigModel(string flowName, GraphQlRequestHandler requestHandler) : base(requestHandler)
        {
            this.flowName = flowName;
        }

        public override Task Delete()
            => RequestHandler.Mutate<JToken>(new(setFlowQuery, new
            {
                flowName,
                source = default(object?),
            }));

        public override async Task<SourceConfig?> Load()
            => (await RequestHandler.Query<Query>(new(getFlowQuery, new
            {
                flowName
            }))).Database?.Flow?.Config;

        public override Task Save(SourceConfig source)
            => RequestHandler.Mutate<JToken>(new(setFlowQuery, new
            {
                flowName,
                source,
            }));
    }
}