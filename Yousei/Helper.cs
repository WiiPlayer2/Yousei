using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei
{
    [DebuggerStepThrough]
    public static class Helper
    {
        public static IAsyncEnumerable<T> YieldAsync<T>(this T item) => item.Yield().ToAsyncEnumerable();

        public static async Task<IAsyncEnumerable<T>> YieldAsync<T>(this Task<T> item) => (await item.ConfigureAwait(false)).YieldAsync();

        public static IEnumerable<T> Yield<T>(this T item) => new[] { item };

        public static async Task<IAsyncEnumerable<T>> ToAsyncEnumerable<T>(this Task<IEnumerable<T>> enumerable) => (await enumerable.ConfigureAwait(false)).ToAsyncEnumerable();

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach(var item in enumerable)
            {
                action(item);
            }
        }

        public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var list = new List<T>();
            await foreach(var item in asyncEnumerable)
            {
                list.Add(item);
            }
            return list;
        }

        public static JToken Get(this JToken token, string str)
        {
            if (str.StartsWith('\\'))
                return str.Substring(1);

            if (str.StartsWith('$'))
                return ResolvePath();

            return str;

            JToken ResolvePath()
            {
                if (str == "$")
                    return token;

                var pathParts = str.Substring(1).Split('.');
                return pathParts.Aggregate(token, (acc, curr) => acc[curr]);
            }
        }

        public static JToken Map(this JToken map, JToken data)
        {
            return map switch
            {
                JValue mapValue => mapValue.Type switch
                {
                    JTokenType.String => data.Get(mapValue.Value<string>()),
                    _ => mapValue,
                },
                JObject mapObject => GetObject(mapObject, data),
                JArray mapArray => GetArray(mapArray, data),
                _ => map,
            };

            JObject GetObject(JObject mapObject, JToken data)
            {
                var newObj = new JObject();
                mapObject.Properties().ForEach(prop => newObj.Add(prop.Name, prop.Value.Map(data)));
                return newObj;
            }

            JArray GetArray(JArray mapArray, JToken data)
            {
                var newArr = new JArray();
                mapArray.ForEach(value => newArr.Add(value.Map(data)));
                return newArr;
            }
        }

        public static TService GetService<TService>(this IServiceProvider serviceProvider) where TService : class
            => serviceProvider.GetService(typeof(TService)) as TService;
    }
}
