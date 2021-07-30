using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal.Connectors.Debug
{
    internal class DumpAction : FlowAction<UnitConnection, Unit>
    {
        private ILogger logger;

        public DumpAction(ILogger logger)
        {
            this.logger = logger;
        }

        public override string Name { get; } = "dump";

        protected override async Task Act(IFlowContext context, UnitConnection _, Unit arguments)
        {
            var contextObj = await context.AsObject();
            var str = GetString(() => contextObj, new HashSet<object>());
            global::System.Diagnostics.Debug.WriteLine(str);
            logger.LogDebug(str);
        }

        private static string GetString(Func<object?> getObj, ISet<object> seenObjects)
        {
            try
            {
                var obj = getObj();
                if (obj is null)
                    return string.Empty;
                if (obj.GetType().IsPrimitive
                    || obj is string
                    || obj is DateTime
                    || obj is DateTimeOffset
                    || obj is JToken
                    || !seenObjects.Add(obj))
                    return obj.ToString() ?? string.Empty;

                if (obj is IEnumerable<byte> byteEnumberable)
                    return GetString(byteEnumberable);
                if (obj is ExpandoObject expandoObject)
                    return GetString(expandoObject, seenObjects);
                if (obj is IEnumerable enumerable)
                    return GetString(enumerable, seenObjects);
                return GetString(obj, seenObjects);
            }
            catch (Exception e)
            {
                return $"[{e.GetType().FullName}: {e.Message}]";
            }
        }

        private static string GetString(IEnumerable<byte> obj)
            => string.Concat(obj.Select(o => o.ToString("X2")));

        private static string GetString(ExpandoObject obj, ISet<object> seenObjects)
        {
            if (obj.Any())
            {
                var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter);
                writer.WriteLine("{");
                using (writer.Indent())
                {
                    writer.WriteLine(string.Join("," + writer.NewLine, obj.Select(kv => $"{kv.Key}: {GetString(() => kv.Value, seenObjects)}")));
                }
                writer.Write("}");
                return stringWriter.ToString();
            }
            else
            {
                return "{}";
            }
        }

        private static string GetString(IEnumerable obj, ISet<object> seenObjects)
        {
            var items = obj.Cast<object?>().ToList();
            if (items.Any())
            {
                var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter);
                writer.WriteLine("[");
                using (writer.Indent())
                {
                    writer.WriteLine(string.Join("," + writer.NewLine, items.Select(v => GetString(() => v, seenObjects))));
                }
                writer.Write("]");
                return stringWriter.ToString();
            }
            else
            {
                return "[]";
            }
        }

        private static string GetString(object obj, ISet<object> seenObjects)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (properties.Any())
            {
                var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter);
                writer.WriteLine($"{type.FullName} {{");
                using (writer.Indent())
                {
                    writer.WriteLine(string.Join("," + writer.NewLine, properties.Select(prop => $"{prop.Name}: {GetString(() => prop.GetValue(obj), seenObjects)}")));
                }
                writer.Write("}");
                return stringWriter.ToString();
            }
            else
            {
                return obj.ToString() ?? type.FullName ?? string.Empty;
            }
        }
    }
}