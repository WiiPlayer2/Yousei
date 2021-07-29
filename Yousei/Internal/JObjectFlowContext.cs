using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal
{
    internal class JObjectFlowContext : IFlowContext
    {
        private readonly JObject data = new JObject();

        public JObjectFlowContext(IFlowActor actor, string flowName)
        {
            Actor = actor;
            this.Flow = flowName;
        }

        private JObjectFlowContext(JObjectFlowContext from)
            : this(from.Actor, from.Flow)
        {
            data = (JObject)from.data.DeepClone();
            ExecutionStack = new Stack<string>(from.ExecutionStack);
            CurrentType = from.CurrentType;
        }

        public IFlowActor Actor { get; }

        public string CurrentType { get; set; } = string.Empty;

        public Stack<string> ExecutionStack { get; } = new Stack<string>();

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
                .Aggregate((JObject?)data, (current, segment) => current?.Value<JObject>(segment));
            relevantObject?.Remove(segments.Last());
        }

        public IFlowContext Clone() => new JObjectFlowContext(this);

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

        public Task<object?> GetData(string path)
        {
            var value = path.SplitPath()
                .Aggregate((JToken?)data, (data, segment) => data?.Value<JToken>(segment));
            return Task.FromResult<object?>(value);
        }

        public Task SetData(string path, object? data)
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException();

            var segments = path.SplitPath();
            SetData(this.data, segments, data);
            return Task.CompletedTask;
        }

        public Task SetData(object? data)
            => SetData(CurrentType, data);

        private void SetData(JToken current, string[] segments, object? data)
        {
            if (segments.Length == 1)
            {
                current[segments[0]] = data.Map<JToken>();
                return;
            }

            var next = current[segments[0]];
            if (next is null || next is not JObject)
            {
                next = new JObject();
                current[segments[0]] = next;
            }
            SetData(next, segments.Skip(1).ToArray(), data);
        }
    }
}