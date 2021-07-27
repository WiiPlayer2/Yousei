using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public static class ObjectConnection
    {
        public static ObjectConnection<T> From<T>(T obj)
            => new ObjectConnection<T>(obj);

        public static DisposableObjectConnection<T> FromDisposable<T>(T obj)
            where T : IDisposable
            => new DisposableObjectConnection<T>(obj);
    }

    public class DisposableObjectConnection<T> : ObjectConnection<T>
        where T : IDisposable
    {
        public DisposableObjectConnection(T obj) : base(obj)
        {
        }

        public override void Dispose()
            => Object.Dispose();
    }

    public class ObjectConnection<T> : IConnection
    {
        public ObjectConnection(T obj)
        {
            Object = obj;
        }

        public T Object { get; }

        public virtual void Dispose()
        {
        }
    }
}