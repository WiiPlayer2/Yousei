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

        public override Task Delete()
            => Database.SetFlow(flowName, null);

        public override async Task<SourceConfig> Load()
        {
            return await Database.GetFlowSource(flowName);
        }

        public override async Task Save(SourceConfig source)
        {
            await Database.SetFlow(flowName, source);
        }
    }
}