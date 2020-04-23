using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class ShellModule : BaseOldModule
    {
        private class Arguments
        {
            public string Command { get; set; }
        }

        public string ID => "shell";

        private bool IsUnix => Environment.OSVersion.Platform == PlatformID.Unix;

        private string ShellCommand => IsUnix ? "sh" : "cmd.exe";

        private string GetShellArgument(string arguments) => IsUnix
            ? $"-c \"{Regex.Escape(arguments)}\""
            : $"/C \"{Regex.Escape(arguments)}\"";

        public override async Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var dataStr = data.ToString();

            var psi = new ProcessStartInfo(ShellCommand, GetShellArgument(args.Command))
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };
            var process = new Process()
            {
                StartInfo = psi,
            };

            process.Start();

            await process.StandardInput.WriteLineAsync(dataStr).ConfigureAwait(false);
            await process.StandardInput.FlushAsync().ConfigureAwait(false);

            process.WaitForExit();

            var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            try
            {
                var resultData = JToken.Parse(output);
                return resultData.YieldAsync();
            }
            catch
            {
                return JValue.CreateString(output).YieldAsync();
            }
        }
    }
}
