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
    internal class AppConfigurationDatabase : IConfigurationDatabase
    {
        private readonly ILogger<AppConfigurationDatabase> logger;

        private readonly GraphQlRequestHandler requestHandler;

        public AppConfigurationDatabase(GraphQlRequestHandler requestHandler, ILogger<AppConfigurationDatabase> logger)
        {
            this.requestHandler = requestHandler;
            this.logger = logger;
        }

        public Task<bool> IsReadOnly => GetIsReadOnly();

        public Task<object?> GetConfiguration(string connector, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceConfig?> GetConfigurationSource(string connector, string name)
        {
            var request = new GraphQLRequest
            {
                Query = @"
query Configuration($connector: String, $name: String) {
  database {
    configuration(connector: $connector, name: $name) {
      config {
        language
        content
      }
    }
  }
}",
                Variables = new
                {
                    connector,
                    name,
                },
            };
            var response = await requestHandler.Query<JToken>(request, logger);
            return response?
                .Value<JToken>("database")?
                .Value<JToken>("configuration")?
                .Value<SourceConfig>("config");
        }

        public Task<FlowConfig?> GetFlow(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceConfig?> GetFlowSource(string name)
        {
            var request = new GraphQLRequest
            {
                Query = @"
query Flow($name: String) {
  database {
    flow(name: $name) {
      config {
        language
        content
      }
    }
  }
}",
                Variables = new
                {
                    name = name,
                },
            };
            var response = await requestHandler.Query<JToken>(request, logger);
            return response?
                .Value<JToken>("database")?
                .Value<JToken>("flow")?
                .Value<SourceConfig>("config");
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations()
        {
            var request = new GraphQLRequest(@"
query {
  database {
    connections {
      id
      configurations {
        name
      }
    }
  }
}");
            var response = await requestHandler.Query<JToken>(request, logger);
            return response?
                .Value<JToken>("database")?
                .Value<List<ConnectionOutput>>("connections")?
                .ToDictionary(o => o.Id, o => (IReadOnlyList<string>)o.Configurations.Select(o => o.Name).ToList())
                ?? new Dictionary<string, IReadOnlyList<string>>();
        }

        public async Task<IReadOnlyList<string>> ListFlows()
        {
            var request = new GraphQLRequest(@"
query {
  database {
    flows {
      name
    }
  }
}");
            var response = await requestHandler.Query<JToken>(request, logger);
            return response?
                .Value<JToken>("database")?
                .Value<List<FlowOutput>>("flows")?
                .Select(o => o.Name)
                .ToList()
                ?? (IReadOnlyList<string>)Array.Empty<string>();
        }

        public async Task SetConfiguration(string connector, string name, SourceConfig? source)
        {
            var request = new GraphQLRequest
            {
                Query = @"
mutation SetConfiguration($connector: String, $name: String, $source: SourceConfigInput) {
  setConfiguration(connector: $connector, name: $name, source: $source) {
    name
  }
}",
                Variables = new
                {
                    connector,
                    name,
                    source,
                },
            };
            await requestHandler.Mutate<JToken>(request, logger);
        }

        public async Task SetFlow(string name, SourceConfig? source)
        {
            var request = new GraphQLRequest
            {
                Query = @"
mutation SetFlow($name: String, $source: SourceConfigInput) {
  setFlow(name: $name, source: $source) {
    name
  }
}",
                Variables = new
                {
                    name,
                    source,
                },
            };
            await requestHandler.Mutate<JToken>(request, logger);
        }

        private async Task<bool> GetIsReadOnly()
        {
            var request = new GraphQLRequest
            {
                Query = @"
query {
    database {
        isReadOnly
    }
}",
            };
            var response = await requestHandler.Query<JToken>(request, logger);
            return response?
                .Value<JToken>("database")?
                .Value<bool>("isReadOnly")
                ?? true;
        }

        private record ConfigurationOutput(string Name);

        private record ConnectionOutput(string Id, ConfigurationOutput[] Configurations);

        private record FlowOutput(string Name);
    }
}