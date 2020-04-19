﻿using Newtonsoft.Json.Linq;
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
    static class Helper
    {
        private class DummyAsyncEnumerable<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> enumerable;

            public DummyAsyncEnumerable(IEnumerable<T> enumerable)
            {
                this.enumerable = enumerable;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator(enumerable.GetEnumerator());

            private class Enumerator : IAsyncEnumerator<T>
            {
                private readonly IEnumerator<T> enumerator;

                public Enumerator(IEnumerator<T> enumerator)
                {
                    this.enumerator = enumerator;
                }

                public T Current => enumerator.Current;

                public ValueTask DisposeAsync() => default;

                public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(enumerator.MoveNext());
            }
        }

        public static IAsyncEnumerable<T> YieldAsync<T>(this T item) => item.Yield().ToAsyncEnumerable();

        public static async Task<IAsyncEnumerable<T>> YieldAsync<T>(this Task<T> item) => (await item.ConfigureAwait(false)).YieldAsync();

        public static IEnumerable<T> Yield<T>(this T item) => new[] { item };

        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable) => new DummyAsyncEnumerable<T>(enumerable);

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

        public static JToken Get(this JToken token, string path)
        {
            var parts = path.Split('.');
            return parts.Aggregate(token, (value, path) => value[path]);
        }
    }
}
