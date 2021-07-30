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
    public class ConnectionConfigModel : ConfigModel
    {
        private readonly string connector;

        private readonly string getConfigurationQuery = @"
query _($connector: String!, $name: String!) {
  database {
    configuration(connector: $connector, name: $name) {
      config {
        content
        language
      }
    }
  }
}";

        private readonly string name;

        private readonly string setConfigurationQuery = @"
mutation _($connector: String!, $name: String!, $source: SourceConfigInput) {
  setConfiguration(connector: $connector, name: $name, source: $source) {
    name
  }
}";

        public ConnectionConfigModel(string connector, string name, GraphQlRequestHandler requestHandler) : base(requestHandler)
        {
            this.connector = connector;
            this.name = name;
        }

        public override Task Delete()
            => RequestHandler.Mutate<JToken>(new(setConfigurationQuery, new
            {
                connector,
                name,
                source = default(object?),
            }));

        public override async Task<SourceConfig?> Load()
            => (await RequestHandler.Query<Query>(new(getConfigurationQuery, new
            {
                connector,
                name
            }))).Database?.Configuration?.Config;

        public override Task Save(SourceConfig source)
            => RequestHandler.Mutate<JToken>(new(setConfigurationQuery, new
            {
                connector,
                name,
                source = source,
            }));
    }
}