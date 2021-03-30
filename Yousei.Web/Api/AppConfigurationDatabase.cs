﻿using GraphQL;
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
    internal class AppConfigurationDatabase : IConfigurationDatabase
    {
        private GraphQLHttpClient client;

        public AppConfigurationDatabase(GraphQLHttpClient client)
        {
            this.client = client;
        }

        public Task<bool> IsReadOnly => GetIsReadOnly();

        public Task<object> GetConfiguration(string connector, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceConfig> GetConfigurationSource(string connector, string name)
        {
            var request = new GraphQLRequest
            {
                Query = @"
query Configuration($connector: String, $name: String) {
  database {
    configuration(connector: $connector, name: $name) {
      config
    }
  }
}",
                Variables = new
                {
                    connector,
                    name,
                },
            };
            var response = await client.SendQueryAsync<JToken>(request);
            return response.Data["database"]["configuration"]["config"].ToObject<SourceConfig>();
        }

        public Task<FlowConfig> GetFlow(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceConfig> GetFlowSource(string name)
        {
            var request = new GraphQLRequest
            {
                Query = @"
query Flow($name: String) {
  database {
    flow(name: $name) {
      config {
        actions {
          type
          configuration
          arguments
        },
        trigger {
          type
          configuration
          arguments
        }
      }
    }
  }
}",
                Variables = new
                {
                    name = name,
                },
            };
            var response = await client.SendQueryAsync<JToken>(request);
            return response.Data["database"]["flow"]["config"].ToObject<SourceConfig>();
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
            var response = await client.SendQueryAsync<JToken>(request);
            return response.Data["database"]["connections"].ToObject<List<ConnectionOutput>>()
                .ToDictionary(o => o.Id, o => (IReadOnlyList<string>)o.Configurations.Select(o => o.Name).ToList());
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
            var response = await client.SendQueryAsync<JToken>(request);
            return response.Data["database"]["flows"].ToObject<List<FlowOutput>>()
                .Select(o => o.Name)
                .ToList();
        }

        public async Task SetConfiguration(string connector, string name, SourceConfig source)
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
            await client.SendMutationAsync<JToken>(request);
        }

        public async Task SetFlow(string name, SourceConfig source)
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
            await client.SendMutationAsync<JToken>(request);
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
            var response = await client.SendQueryAsync<JToken>(request);
            return response.Data["database"]["isReadOnly"].ToObject<bool>();
        }

        private record ConfigurationOutput(string Name);

        private record ConnectionOutput(string Id, ConfigurationOutput[] Configurations);

        private record FlowOutput(string Name);
    }
}