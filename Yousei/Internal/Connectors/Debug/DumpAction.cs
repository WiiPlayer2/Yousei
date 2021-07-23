using Microsoft.Extensions.Logging;
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
    internal class DumpAction : FlowAction<Unit>
    {
        private ILogger logger;

        public DumpAction(ILogger logger)
        {
            this.logger = logger;
        }

        protected override async Task Act(IFlowContext context, Unit arguments)
        {
            var contextObj = await context.AsObject();
            var str = GetString(() => contextObj);
            global::System.Diagnostics.Debug.WriteLine(str);
            logger.LogDebug(str);
        }

        private static string GetString(Func<object?> getObj)
        {
            try
            {
                var obj = getObj();
                if (obj is null)
                    return string.Empty;
                if (obj.GetType().IsPrimitive || obj is string)
                    return obj.ToString() ?? string.Empty;
                if (obj is ExpandoObject expandoObject)
                    return GetString(expandoObject);
                if (obj is IEnumerable enumerable)
                    return GetString(enumerable);

                var str = obj.ToString();
                if (str != obj.GetType().FullName)
                    return str ?? string.Empty;

                return GetString(obj);
            }
            catch (Exception e)
            {
                return $"[{e.GetType().FullName}: {e.Message}]";
            }
        }

        private static string GetString(ExpandoObject obj)
        {
            if (obj.Any())
            {
                var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter);
                writer.WriteLine("{");
                using (writer.Indent())
                {
                    writer.WriteLine(string.Join("," + writer.NewLine, obj.Select(kv => $"{kv.Key}: {GetString(() => kv.Value)}")));
                }
                writer.Write("}");
                return stringWriter.ToString();
            }
            else
            {
                return "{}";
            }
        }

        private static string GetString(IEnumerable obj)
        {
            var items = obj.Cast<object?>().ToList();
            if (items.Any())
            {
                var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter);
                writer.WriteLine("[");
                using (writer.Indent())
                {
                    writer.WriteLine(string.Join("," + writer.NewLine, items.Select(v => GetString(() => v))));
                }
                writer.Write("]");
                return stringWriter.ToString();
            }
            else
            {
                return "[]";
            }
        }

        private static string GetString(object obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();
            if (properties.Any())
            {
                var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter);
                writer.WriteLine($"{type.FullName} {{");
                using (writer.Indent())
                {
                    writer.WriteLine(string.Join("," + writer.NewLine, properties.Select(prop => $"{prop.Name}: {GetString(() => prop.GetValue(obj))}")));
                }
                writer.Write("}");
                return stringWriter.ToString();
            }
            else
            {
                return type.FullName ?? obj.ToString() ?? string.Empty;
            }
        }
    }
}