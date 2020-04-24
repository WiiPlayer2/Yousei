using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Modules.Templates;
using static LanguageExt.Prelude;

namespace Yousei.Modules
{
    public class FilterModule : OptionTemplate
    {
        private enum FilterType
        {
            Regex,
            Simple,
        }

        private class Arguments
        {
            public string Path { get; set; }

            public FilterType Type { get; set; }

            public string Pattern { get; set; }
        }

        private class SimpleRegex : Regex
        {
            public SimpleRegex(string pattern)
            : base(GetPattern(pattern), RegexOptions.IgnoreCase) { }

            private static string GetPattern(string pattern)
            {
                var escapedPattern = Escape(pattern);
                return escapedPattern.Replace("\\*", ".*").Replace("\\?", ".?");
            }
        }

        public override OptionAsync<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var value = data.Get(args.Path).ToObject<string>();
            var regex = args.Type switch
            {
                FilterType.Regex => new Regex(args.Pattern),
                FilterType.Simple => new SimpleRegex(args.Pattern),
                _ => throw new NotSupportedException(),
            };

            if (regex.IsMatch(value))
                return SomeAsync(data);
            return None;
        }
    }
}
