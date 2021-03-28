using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Web.Model
{
    public class FlowConfigModel : ConfigModel
    {
        private readonly string flowName;

        public FlowConfigModel(string flowName, IConfigurationDatabase database) : base(database)
        {
            this.flowName = flowName;
        }

        public override async Task<string> Load()
        {
            var config = await Database.GetFlow(flowName);
            return config.Map<JToken>().ToString();
        }

        public override async Task Save(string content)
        {
            var config = JToken.Parse(content).ToObject<FlowConfig>();
            await Database.SetFlow(flowName, config);
        }
    }
}