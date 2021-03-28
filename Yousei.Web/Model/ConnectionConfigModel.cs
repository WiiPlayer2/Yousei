using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Web.Model
{
    public class ConnectionConfigModel : ConfigModel
    {
        private readonly string connector;

        private readonly string name;

        public ConnectionConfigModel(string connector, string name, IConfigurationDatabase database) : base(database)
        {
            this.connector = connector;
            this.name = name;
        }

        public override Task Delete()
            => Database.SetConfiguration(connector, name, null);

        public override async Task<string> Load()
        {
            var config = await Database.GetConfiguration(connector, name);
            return config.Map<JToken>().ToString();
        }

        public override async Task Save(string content)
        {
            var config = JToken.Parse(content);
            await Database.SetConfiguration(connector, name, config);
        }
    }
}