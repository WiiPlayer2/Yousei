using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal
{
    internal class FlowContext : IFlowContext
    {
        private readonly JObject data = new JObject();

        public FlowContext(IFlowActor actor, string flowName)
        {
            Actor = actor;
            this.Flow = flowName;
        }

        private FlowContext(FlowContext from)
            : this(from.Actor, from.Flow)
        {
            data = from.data.DeepClone() as JObject;
        }

        public IFlowActor Actor { get; }

        public string CurrentType { get; set; }

        public string Flow { get; }

        public Task<object> AsObject() => Task.FromResult<object>(data);

        public async Task ClearData(string path)
        {
            var exists = await ExistsData(path);
            if (!exists)
                return;

            var segments = path.SplitPath();
            var relevantObject = segments
                .Take(segments.Length - 1)
                .Aggregate(data, (current, segment) => current[segment] as JObject);
            relevantObject.Remove(segments.Last());
        }

        public IFlowContext Clone() => new FlowContext(this);

        public Task<bool> ExistsData(string path)
        {
            var segments = path.SplitPath();
            JToken current = data;
            foreach (var segment in segments)
            {
                if (current[segment] is not JToken newToken)
                    return Task.FromResult(false);
                current = newToken;
            }

            return Task.FromResult(true);
        }

        public Task<object> GetData(string path)
        {
            var value = path.SplitPath()
                .Aggregate(data as JToken, (data, segment) => data[segment]);
            return Task.FromResult<object>(value);
        }

        public Task SetData(string path, object data)
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException();

            var segments = path.SplitPath();
            JToken current = this.data;
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (i < segments.Length - 1)
                {
                    if (current[segment] is not JObject)
                        current[segment] = new JObject();
                    current = current[segment];
                    continue;
                }

                current[segment] = data.Map<JToken>();
            }
            return Task.CompletedTask;
        }

        public Task SetData(object data)
            => SetData(CurrentType, data);
    }
}