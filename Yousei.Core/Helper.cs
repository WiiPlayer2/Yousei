using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public static class Helper
    {
        public static string GetStackTrace(this IFlowContext context)
            => string.Join("\n", context.ExecutionStack.Select(o => $"@ {o}"));

        public static Type GetValueType(this Type parameterType)
        {
            if (!parameterType.IsAssignableTo(typeof(IParameter)))
                throw new ArgumentException();

            var interfaceTypes = parameterType.GetInterfaces().ToList();
            if (parameterType.IsInterface)
                interfaceTypes.Add(parameterType);

            var genericInterface = interfaceTypes
                .FirstOrDefault(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IParameter<>));

            return genericInterface is not null
                ? genericInterface.GenericTypeArguments[0]
                : typeof(object);
        }

        public static async void Ignore(this Task task)
        {
            try
            {
                await task;
            }
            catch { }
        }

        public static async Task IgnoreCancellation(this Task task, CancellationToken? cancellationToken = default)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException e) when (cancellationToken is null || e.CancellationToken == cancellationToken) { }
        }

        public static TTarget? Map<TTarget>(this object? source)
            => (TTarget?)source.Map(typeof(TTarget));

        public static object? Map(this object? source, Type targetType)
            => source is null ? null : JToken.FromObject(source).ToObject(targetType);

        public static IParameter Map(this IParameter parameter, Type targetType)
        {
            if (targetType == typeof(object))
                return parameter;

            var mappedParameterType = typeof(MappedParameter<>).MakeGenericType(targetType);
            var constructor = mappedParameterType.GetConstructor(new[] { typeof(IParameter) });
            if (constructor is null)
                throw new InvalidOperationException();

            var expression = Expression.Lambda<Func<IParameter>>(Expression.New(constructor, Expression.Constant(parameter)));
            return expression.Compile()();
        }

        public static IParameter<T> Map<T>(this IParameter parameter)
            => (IParameter<T>)parameter.Map(typeof(T));

        public static T? SafeCast<T>(this object? obj)
            => obj is T castObj ? castObj : default;

        public static IDisposable ScopeStack(this IFlowContext context, string frameDescription)
        {
            context.ExecutionStack.Push(frameDescription);
            return new ActionDisposable(() => context.ExecutionStack.Pop());
        }

        public static IEnumerable<Task<TOut>> Select<TIn, TOut>(this IEnumerable<Task<TIn>> sequence, Func<TIn, TOut> selector)
            => Enumerable.Select(sequence, async o => selector(await o));

        public static string[] SplitPath(this string s)
        {
            var splits = s.Split('.');
            return splits;
        }

        public static (string ConnectorName, string Name) SplitType(this string s)
        {
            var splits = s.Split('.');
            if (splits.Length != 2)
                throw new ArgumentException($"\"{s}\" is not valid type.", nameof(s));

            return (splits[0], splits[1]);
        }

        public static void ThrowIfNull<T>([NotNull] this T? value)
        {
            if (value is null)
                throw new ArgumentNullException();
        }

        public static ConstantParameter<T> ToConstantParameter<T>(this T obj)
            => new ConstantParameter<T>(obj);

        public static async Task<List<T>> ToList<T>(this IEnumerable<Task<T>> sequence)
        {
            var list = new List<T>();
            foreach (var item in sequence)
            {
                list.Add(await item);
            }
            return list;
        }

        public static bool TryGetValue(this IDictionary dictionary, object key, out object? value)
        {
            if (dictionary.Contains(key))
            {
                value = dictionary[key];
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryToObject<T>(this JToken jtoken, [NotNullWhen(true)] out T? result)
            where T : notnull
        {
            try
            {
                result = jtoken.ToObject<T>();
                return result is not null;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}