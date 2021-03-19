using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public static class Helper
    {
        public static TTarget Map<TTarget>(this object source)
            => (TTarget)source.Map(typeof(TTarget));

        public static object Map(this object source, Type targetType)
            => source is null ? null : JToken.FromObject(source).ToObject(targetType);

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
    }
}