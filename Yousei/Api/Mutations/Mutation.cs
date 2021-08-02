using HotChocolate;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Mutations
{
    public class Mutation
    {
        public async Task<JToken> Act(
            IEnumerable<BlockConfigInput> config,
            [Service] IFlowActor actor,
            [Service] IFlowContextFactory flowContextFactory)
        {
            var context = flowContextFactory.Create(actor, "<GRAPHQL>");
            await actor.Act(config.Select(o => o.Map()).ToList(), context);
            var obj = await context.AsObject();

            if (obj is JToken jObj)
                return jObj;
            return JToken.FromObject(obj);
        }

        public Unit Reload(
            [Service] IServiceProvider serviceProvider)
        {
            var eventHub = (EventHub)(serviceProvider.GetService(typeof(EventHub)) ?? throw new InvalidOperationException());
            eventHub.TriggerReload();
            return Unit.Default;
        }
    }
}