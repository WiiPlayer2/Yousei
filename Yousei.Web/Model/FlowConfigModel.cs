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
        private readonly string flowName;

        public FlowConfigModel(string flowName, bool isReadOnly, YouseiApi api) : base(isReadOnly, api)
        {
            this.flowName = flowName;
        }

        public override Task Delete()
            => Api.SetFlow.ExecuteAsync(flowName, null);

        public override async Task<SourceConfig?> Load()
        {
            var result = (await Api.GetFlow.ExecuteAsync(flowName)).Data?.Database.Flow?.Config;
            if (result is null)
                return null;
            return new(result.Language, result.Content);
        }

        public override Task Save(SourceConfig source)
            => Api.SetFlow.ExecuteAsync(flowName, new()
            {
                Content = source.Content,
                Language = source.Language,
            });
    }
}