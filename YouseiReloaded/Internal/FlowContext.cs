using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal
{
    internal class FlowContext : IFlowContext
    {
        private readonly JObject data = new JObject();

        public FlowContext(IFlowActor actor)
        {
            Actor = actor;
        }

        private FlowContext(FlowContext from)
            : this(from.Actor)
        {
            data = from.data.DeepClone() as JObject;
        }

        public IFlowActor Actor { get; }

        public Task AddData(string type, object data)
        {
            var (connectorName, name) = type.SplitType();
            if (!this.data.ContainsKey(connectorName))
                this.data[connectorName] = new JObject();
            this.data[connectorName][name] = data.Map<JToken>();
            return Task.CompletedTask;
        }

        public Task<object> AsObject() => Task.FromResult<object>(data);

        public IFlowContext Clone() => new FlowContext(this);

        public Task<object> GetData(string path)
        {
            var value = path.SplitPath()
                .Aggregate(data as JToken, (data, segment) => data[segment]);
            return Task.FromResult<object>(value);
        }
    }
}