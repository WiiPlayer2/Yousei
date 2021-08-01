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

        private readonly string name;

        public ConnectionConfigModel(string connector, string name, bool isReadOnly, YouseiApi api) : base(isReadOnly, api)
        {
            this.connector = connector;
            this.name = name;
        }

        public override Task Delete()
            => Api.SetConfiguration.ExecuteAsync(connector, name, null);

        public override async Task<SourceConfig?> Load()
        {
            var result = (await Api.GetConfiguration.ExecuteAsync(connector, name)).Data?.Database.Configuration?.Config;
            if (result is null)
                return null;

            return new(result.Language, result.Content);
        }

        public override Task Save(SourceConfig source)
            => Api.SetConfiguration.ExecuteAsync(connector, name, new()
            {
                Content = source.Content,
                Language = source.Language,
            });
    }
}