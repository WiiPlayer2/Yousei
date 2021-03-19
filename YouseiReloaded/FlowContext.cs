using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace YouseiReloaded
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

        public Task AddData(string type, JToken data)
        {
            var (connectorName, name) = type.SplitType();
            if (!this.data.ContainsKey(connectorName))
                this.data[connectorName] = new JObject();
            this.data[connectorName][name] = data;
            return Task.CompletedTask;
        }

        public Task<JObject> AsObject() => Task.FromResult(data);

        public IFlowContext Clone() => new FlowContext(this);

        public Task<JToken> GetData(string path)
        {
            var value = path.SplitPath()
                .Aggregate(data as JToken, (data, segment) => data[segment]);
            return Task.FromResult(value);
        }
    }
}