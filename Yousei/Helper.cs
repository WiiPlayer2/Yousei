using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei
{
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

        public static IEnumerable<T> Yield<T>(this T item) => new[] { item };

        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable) => new DummyAsyncEnumerable<T>(enumerable);
    }
}
