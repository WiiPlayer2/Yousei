using Microsoft.Extensions.Logging;
using System;
using System.CodeDom.Compiler;
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
            var str = GetStringRepresentation(contextObj);
            global::System.Diagnostics.Debug.WriteLine(str);
            logger.LogDebug(str);
        }

        private string GetStringRepresentation(object? obj)
        {
            var stringWriter = new StringWriter();
            var writer = new IndentedTextWriter(stringWriter);
            WriteString(obj);
            writer.Flush();
            return stringWriter.ToString();

            void WriteString(object? obj)
            {
                if (obj is null)
                    return;

                try
                {
                    try
                    {
                        var str = obj.ToString();
                        if (str != obj.GetType().FullName)
                        {
                            writer.WriteLine(str);
                            return;
                        }
                    }
                    catch { }

                    if (obj is ExpandoObject && obj is IDictionary<string, object?> expandoDict)
                    {
                        writer.WriteLine("{");

                        // Indent
                        writer.Indent++;
                        foreach (var (key, value) in expandoDict)
                        {
                            writer.Write($"{key}: ");
                            WriteString(value);
                        }
                        // Unindent
                        writer.Indent--;

                        writer.WriteLine("}");
                    }
                    else
                    {
                        writer.WriteLine(obj.GetType().FullName);
                    }
                }
                catch (Exception e)
                {
                    writer.WriteLine($"[{e.GetType().FullName}: {e.Message}]");
                }
            }
        }
    }
}