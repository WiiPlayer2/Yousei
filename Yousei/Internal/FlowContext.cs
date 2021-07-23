using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Yousei.Internal
{
    internal class FlowContext : IFlowContext
    {
        private ExpandoObject data = new ExpandoObject();

        public FlowContext(IFlowActor actor, string flowName)
        {
            Actor = actor;
            Flow = flowName;
        }

        private FlowContext(FlowContext from)
            : this(from.Actor, from.Flow)
        {
            data = Clone(from.data);
            ExecutionStack = new Stack<string>(from.ExecutionStack);
        }

        public IFlowActor Actor { get; }

        public string CurrentType { get; set; } = string.Empty;

        public Stack<string> ExecutionStack { get; } = new Stack<string>();

        public string Flow { get; }

        public Task<object> AsObject() => Task.FromResult<object>(data);

        public Task ClearData(string path)
        {
            var segments = path.SplitPath();
            Set(data, segments, null);
            return Task.CompletedTask;
        }

        public IFlowContext Clone() => new FlowContext(this);

        public Task<bool> ExistsData(string path)
        {
            var segments = path.SplitPath();
            return Task.FromResult(Exists(data, segments));
        }

        public Task<object?> GetData(string path)
        {
            var segments = path.SplitPath();
            return Task.FromResult(Get(data, segments));
        }

        public Task SetData(object? data)
            => SetData(CurrentType, data);

        public Task SetData(string path, object? data)
        {
            var segments = path.SplitPath();
            Set(this.data, segments, data);
            return Task.CompletedTask;
        }

        private ExpandoObject Clone(ExpandoObject obj)
        {
            var ret = new ExpandoObject();
            var retDict = ret as IDictionary<string, object?>;
            foreach (var (key, value) in obj)
            {
                retDict[key] = value is ExpandoObject expandoValue ? Clone(expandoValue) : value;
            }
            return ret;
        }

        private bool Exists(object obj, string[] path)
        {
            if (path.Length == 0)
                return Get(obj, path[0]).Exists;

            var (exists, next) = Get(obj, path[0]);
            if (!exists || next is null)
                return false;

            return Exists(next, path.Skip(1).ToArray());
        }

        private object? Get(object obj, string[] path)
        {
            if (path.Length == 1)
                return Get(obj, path[0]).Value;

            var (_, next) = Get(obj, path[0]);
            if (next is null)
                throw new ArgumentException($"Path is not fully resolvable because an intermediate value is null.");

            return Get(next, path.Skip(1).ToArray());
        }

        private (bool Exists, object? Value) Get(object obj, string property)
        {
            switch (obj)
            {
                case IDynamicMetaObjectProvider dmop:
                    {
                        var expression = Expression.Constant(dmop);
                        var dmo = dmop.GetMetaObject(expression);
                        var memberNames = dmo.GetDynamicMemberNames().ToList();
                        if (!memberNames.Contains(property))
                            return (false, default);

                        var binder = Binder.GetMember(CSharpBinderFlags.None, property, null, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                        var getExpression = Expression.Lambda<Func<object>>(Expression.Dynamic(binder, typeof(object), Expression.Constant(dmop)));
                        var result = getExpression.Compile()();
                        return (true, result);
                    }

                case IDictionary dictionary:
                    {
                        var result = dictionary.TryGetValue(property, out var value);
                        return (result, value);
                    }

                default:
                    {
                        var type = obj.GetType();
                        var member = type.GetMember(property, BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).SingleOrDefault();
                        if (member is null)
                            return (false, default);
                        else if (member is FieldInfo fieldInfo)
                            return (true, fieldInfo.GetValue(obj));
                        else if (member is PropertyInfo propertyInfo)
                            return (true, propertyInfo.GetValue(obj));

                        throw new NotImplementedException();
                    }
            }
        }

        private void Set(object obj, string[] path, object? value)
        {
            if (path.Length == 1)
            {
                Set(obj, path[0], value);
                return;
            }

            var (exists, next) = Get(obj, path[0]);
            if (!exists)
            {
                next = new ExpandoObject();
                Set(obj, path[0], next);
            }
            else if (next is null)
            {
                throw new ArgumentException($"Path is not fully resolvable because an intermediate value is null.");
            }

            Set(next, path.Skip(1).ToArray(), value);
        }

        private void Set(object obj, string property, object? value)
        {
            switch (obj)
            {
                case IDynamicMetaObjectProvider provider:
                    {
                        var binder = Binder.SetMember(CSharpBinderFlags.None, property, default, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                        var expression = Expression.Lambda<Action>(Expression.Dynamic(binder, typeof(object), Expression.Constant(provider), Expression.Constant(value)));
                        expression.Compile()();
                        break;
                    }

                case IDictionary dictionary:
                    {
                        dictionary[property] = value;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}